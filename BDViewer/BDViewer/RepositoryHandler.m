//
//  RepositoryHandler.m
//  BDViewer
//
//  Created by Karl Schulze on 2011-08-24.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "RepositoryHandler.h"
#import "RepositoryConstants.h"
#import "BDQueueEntry.h"
#import "BDLinkedNote.h"
#import "BDSection.h"
#import "BDCategory.h"
#import "BDSubcategory.h"
#import "BDDisease.h"
#import "BDPresentation.h"
#import "BDTherapyGroup.h"
#import "BDTherapy.h"

#import <AWSiOSSDK/SimpleDB/AmazonSimpleDBClient.h>
#import "SdbRequestDelegate.h"

@interface RepositoryHandler()

-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithSection:(BDSection *)section;
-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithCategory:(BDCategory *)category;
-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithSubcategory:(BDSubcategory *)subcategory;
-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithDisease:(BDDisease *)disease;
-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithPresentation:(BDPresentation *)presentation;
-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithTherapyGroup:(BDTherapyGroup *)therapyGroup;
-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithTherapy:(BDTherapy *)therapy;
-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithLinkedNote:(BDLinkedNote *)linkedNote;
-(int)pushQueuedEntries;
-(NSString *)loadEntityWithItemName:(NSString *)theItemName forDomain:(NSString *)theDomain;
-(NSString *)getSimpleSelectExpressionForDomain:(NSString *)theDomain;
-(NSString *)getSelectExpressionForDomain:(NSString *)theDomain withDateItem:(NSString *)theDateItem beforeDate:(NSString *) theCompareDate;

@end

@implementation RepositoryHandler

+(int)pushQueue
{
    RepositoryHandler *handler = [[RepositoryHandler alloc] init];
    
    int processedCount = [handler pushQueuedEntries];
    
    [handler release];
    
    return processedCount;
}

+(void)pullAll
{
    RepositoryHandler *handler = [[RepositoryHandler alloc] init];
    
    NSArray *entityArray = [[NSArray alloc] initWithObjects:DOMAIN_SECTION, DOMAIN_CATEGORY, DOMAIN_SUBCATEGORY, DOMAIN_DISEASE, DOMAIN_PRESENTATION, DOMAIN_THERAPYGROUP, DOMAIN_THERAPY, DOMAIN_LINKEDNOTE, nil];
    
    for (int i = 0; i < [entityArray count]; i++)
    {
        NSString *selectExpression = [handler getSimpleSelectExpressionForDomain:[entityArray objectAtIndex:i]];
        @try 
        {
            SimpleDBSelectRequest  *selectRequest  = [[[SimpleDBSelectRequest alloc] initWithSelectExpression:selectExpression] autorelease];
            SimpleDBSelectResponse *selectResponse = [[RepositoryConstants sdb] select:selectRequest];
            
            for (SimpleDBItem *item in selectResponse.items) 
            {
                NSString *loadedUUID = [handler loadEntityWithItemName:item.name forDomain:DOMAIN_LINKEDNOTE];
                NSLog(@"Loaded %@", loadedUUID);
            }
            
        }
        @catch (AmazonServiceException *exception) 
        {
            NSLog(@"Exception = %@", exception);
        }
                                      
    }
    [handler release];
    [entityArray release];
    
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];

    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    [defaults setObject:[dateFormatter stringFromDate:[NSDate date]] forKey:@"RepositoryRefreshDate"];
    
    [dateFormatter release];

}

+(void)pullLatest
{
    RepositoryHandler *handler = [[RepositoryHandler alloc] init];

//    NSArray *entityArray = [[NSArray alloc] initWithObjects:DOMAIN_SECTION, DOMAIN_CATEGORY, DOMAIN_SUBCATEGORY, DOMAIN_DISEASE, DOMAIN_PRESENTATION, DOMAIN_THERAPYGROUP, DOMAIN_THERAPY, DOMAIN_LINKEDNOTE, nil];

    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];

    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    NSObject *refreshDateInfo = [defaults objectForKey:@"RepositoryRefreshDate"];
    NSString *refreshDateString = nil;
    
    if(nil != refreshDateInfo)
    {
        refreshDateString = (NSString *)refreshDateInfo;
    }
    
    NSLog(@"Last Refresh: %@", refreshDateString);
    
    NSString *selectExpression;
    
    //TODO: EXPAND FOR ALL ENTITIES
    
    // EditorDocuments

    if(nil == refreshDateString)
    {
        selectExpression = [handler getSimpleSelectExpressionForDomain: DOMAIN_LINKEDNOTE];
    }
    else
    {
        selectExpression = [handler getSelectExpressionForDomain:DOMAIN_LINKEDNOTE withDateItem:LN_MODIFIEDDATE beforeDate: refreshDateString];
    }
    @try 
    {
        SimpleDBSelectRequest  *selectRequest  = [[[SimpleDBSelectRequest alloc] initWithSelectExpression:selectExpression] autorelease];
        SimpleDBSelectResponse *selectResponse = [[RepositoryConstants sdb] select:selectRequest];
        
        NSLog(@"Number of items to load:%d", [selectResponse.items count]);
        
        for (SimpleDBItem *item in selectResponse.items) 
        {            
            BDLinkedNote *document = [BDLinkedNote retrieveWithUUID:[handler loadEntityWithItemName:item.name forDomain:DOMAIN_LINKEDNOTE]];
            
            if ((nil != document) && (nil != document.storageKey) && ([document.storageKey length] > 0))
            {
                NSLog(@"Retrieving S3 Doc:%@", document.storageKey);
                
                @try 
                {
                    S3GetObjectRequest  *s3ObjectRequest  = [[S3GetObjectRequest alloc] initWithKey:document.storageKey 
                                                                                           withBucket:BUCKET_LINKEDNOTE];
                    
                    S3GetObjectResponse *s3ObjectResponse = [[RepositoryConstants s3] getObject:s3ObjectRequest];
                    
                    //Expects that this will be text
                    NSLog(@"S3 content type = %@", s3ObjectResponse.contentType);
                    
                    if ([s3ObjectResponse.contentType isEqualToString:@"text/html"] || [s3ObjectResponse.contentType isEqualToString:@"text/plain"]) 
                    {
                        NSString *documentText = [[NSString alloc] initWithData:s3ObjectResponse.body encoding:NSUTF8StringEncoding];
                        
                        NSLog(@"%@", documentText);
                        document.documentText = documentText;
                        [[DataController sharedInstance] saveContext];
                    }                    
                }
                @catch (AmazonServiceException *exception) 
                {
                    NSLog(@"Exception = %@", exception);
                }
            }
        }
    }
    @catch (AmazonServiceException *exception) 
    {
        NSLog(@"Exception = %@", exception);
    }
    
    [handler release];

    [defaults setObject:[dateFormatter stringFromDate:[NSDate date]] forKey:@"RepositoryRefreshDate"];
    
    [dateFormatter release];
}

-(int)pushQueuedEntries
{
    NSMutableArray *queueEntries = [[DataController sharedInstance] allInstancesOf:@"QueueEntry" 
                                                                         orderedBy:nil 
                                                                          loadData:NO 
      
                                                                         targetMOC:nil];
    
//    sdbDelegate = [[SdbRequestDelegate alloc] init];

    int processCount = 0;
    while([queueEntries count] > 0)
    {
        BDQueueEntry *queueEntry = [queueEntries objectAtIndex:0];
        
        if ([queueEntry.objectEntityName isEqualToString:ENTITYNAME_LINKEDNOTE]) 
        {
            BDLinkedNote *document = [BDLinkedNote retrieveWithUUID:queueEntry.objectUuid];
            
            document.inUseBy = @"";
            
            SimpleDBPutAttributesRequest *sdbPutRequest = [self sdbPutAttributeRequestWithLinkedNote:document];
            [[RepositoryConstants sdb] putAttributes:sdbPutRequest];
            
            // Put the file as an object in the bucket.

            S3PutObjectRequest *putObjectRequest = [[S3PutObjectRequest alloc] initWithKey:document.storageKey 
                                                                                  inBucket:BUCKET_LINKEDNOTE];
            putObjectRequest.contentType = @"text/plain";
            
            putObjectRequest.data = [document.documentText dataUsingEncoding:NSUTF8StringEncoding];
            
            [[RepositoryConstants s3] putObject:putObjectRequest];
            
            processCount++;
            
            [[[DataController sharedInstance] managedObjectContext] deleteObject:queueEntry];
            [queueEntries removeObjectAtIndex:0];
            [putObjectRequest release];
        }
        else
        {
            NSLog(@"Unhandled queue entry [%@]", queueEntry.objectEntityName);
            break;
        }
    }
    if (processCount > 0)
    {
        [[DataController sharedInstance] saveContext];
    }
    return processCount;
}

#pragma mark - SimpleDB Put Requests

////  CREATE ONE OF THESE FOR EACH ENTITY >>>>

-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithSection:(BDSection *)section
{
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *itemName = section.uuid;
    NSLog(@"ItemName to push:%@", itemName);
    NSMutableArray *attributes = [[NSMutableArray alloc] initWithCapacity:8];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SN_UUID 
                                                                     andValue:section.uuid 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SN_SCHEMAVERSION 
                                                                     andValue:[NSString stringWithFormat:@"%d", [section.schemaVersion intValue]]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SN_INUSEBY 
                                                                     andValue:(nil == section.inUseBy) ? @"" : section.inUseBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SN_CREATEDDATE 
                                                                     andValue:[dateFormatter stringFromDate:section.createdDate] 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SN_CREATEDBY 
                                                                     andValue:(nil == section.createdBy) ? @"" : section.createdBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SN_MODIFIEDDATE 
                                                                     andValue:[dateFormatter stringFromDate:section.modifiedDate] 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SN_MODIFIEDBY 
                                                                     andValue:(nil == section.modifiedBy) ? @"" : section.modifiedBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SN_DEPRECATED 
                                                                     andValue:[Utility nsNumberBoolToString:section.deprecated]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SN_NAME
                                                                      andValue:section.name
                                                                    andReplace:YES] autorelease]];
    
    SimpleDBPutAttributesRequest *sdbPutRequest = [[SimpleDBPutAttributesRequest alloc] 
                                                   initWithDomainName:DOMAIN_SECTION 
                                                   andItemName:itemName 
                                                   andAttributes:attributes];
    
    [attributes release];
    [dateFormatter release];
    return [sdbPutRequest autorelease];
}

-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithCategory:(BDCategory *)category
{
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *itemName = category.uuid;
    NSLog(@"ItemName to push:%@", itemName);
    NSMutableArray *attributes = [[NSMutableArray alloc] initWithCapacity:8];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:CT_UUID 
                                                                     andValue:category.uuid 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:CT_SCHEMAVERSION 
                                                                     andValue:[NSString stringWithFormat:@"%d", [category.schemaVersion intValue]]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:CT_INUSEBY 
                                                                     andValue:(nil == category.inUseBy) ? @"" : category.inUseBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:CT_CREATEDDATE 
                                                                     andValue:[dateFormatter stringFromDate:category.createdDate] 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:CT_CREATEDBY 
                                                                     andValue:(nil == category.createdBy) ? @"" : category.createdBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:CT_MODIFIEDDATE 
                                                                     andValue:[dateFormatter stringFromDate:category.modifiedDate] 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:CT_MODIFIEDBY 
                                                                     andValue:(nil == category.modifiedBy) ? @"" : category.modifiedBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:CT_DEPRECATED 
                                                                     andValue:[Utility nsNumberBoolToString:category.deprecated]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:CT_SECTIONID
                                                                     andValue:category.sectionId
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:CT_NAME
                                                                     andValue:category.name
                                                                   andReplace:YES] autorelease]];
    
    SimpleDBPutAttributesRequest *sdbPutRequest = [[SimpleDBPutAttributesRequest alloc] 
                                                   initWithDomainName:DOMAIN_CATEGORY 
                                                   andItemName:itemName 
                                                   andAttributes:attributes];
    
    [attributes release];
    [dateFormatter release];
    return [sdbPutRequest autorelease];
}

-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithSubcategory:(BDSubcategory *)subcategory
    {
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
    [dateFormatter setTimeStyle:NSDateFormatterFullStyle];
    [dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *itemName = subcategory.uuid;
    NSLog(@"ItemName to push:%@", itemName);
    NSMutableArray *attributes = [[NSMutableArray alloc] initWithCapacity:8];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SC_UUID 
                                                                     andValue:subcategory.uuid 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SC_SCHEMAVERSION 
                                                                     andValue:[NSString stringWithFormat:@"%d", [subcategory.schemaVersion intValue]]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SC_INUSEBY 
                                                                     andValue:(nil == subcategory.inUseBy) ? @"" : subcategory.inUseBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SC_CREATEDDATE 
                                                                     andValue:[dateFormatter stringFromDate:subcategory.createdDate] 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SC_CREATEDBY 
                                                                     andValue:(nil == subcategory.createdBy) ? @"" : subcategory.createdBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SC_MODIFIEDDATE 
                                                                     andValue:[dateFormatter stringFromDate:subcategory.modifiedDate] 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SC_MODIFIEDBY 
                                                                     andValue:(nil == subcategory.modifiedBy) ? @"" : subcategory.modifiedBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SC_DEPRECATED 
                                                                     andValue:[Utility nsNumberBoolToString:subcategory.deprecated]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SC_CATEGORYID                         
                                                                     andValue:subcategory.categoryId
                                                                   andReplace:YES] autorelease]];
     
     [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SC_NAME
                                                                      andValue:subcategory.name
                                                                    andReplace:YES] autorelease]];
     
     SimpleDBPutAttributesRequest *sdbPutRequest = [[SimpleDBPutAttributesRequest alloc] 
                                                    initWithDomainName:DOMAIN_SUBCATEGORY 
                                                    andItemName:itemName 
                                                    andAttributes:attributes];
     
     [attributes release];
     [dateFormatter release];
     return [sdbPutRequest autorelease];
 }
         
-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithDisease:(BDDisease *)disease
{
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
    [dateFormatter setTimeStyle:NSDateFormatterFullStyle];
    [dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *itemName = disease.uuid;
    NSLog(@"ItemName to push:%@", itemName);
    NSMutableArray *attributes = [[NSMutableArray alloc] initWithCapacity:8];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:DI_UUID 
                                                                     andValue:disease.uuid 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:DI_SCHEMAVERSION 
                                                                     andValue:[NSString stringWithFormat:@"%d", [disease.schemaVersion intValue]]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:DI_INUSEBY 
                                                                     andValue:(nil == disease.inUseBy) ? @"" : disease.inUseBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:DI_CREATEDDATE 
                                                                     andValue:[dateFormatter stringFromDate:disease.createdDate] 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:DI_CREATEDBY 
                                                                     andValue:(nil == disease.createdBy) ? @"" : disease.createdBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:DI_MODIFIEDDATE 
                                                                     andValue:[dateFormatter stringFromDate:disease.modifiedDate] 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:DI_MODIFIEDBY 
                                                                     andValue:(nil == disease.modifiedBy) ? @"" : disease.modifiedBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:DI_DEPRECATED 
                                                                     andValue:[Utility nsNumberBoolToString:disease.deprecated]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:DI_CATEGORYID                         
                                                                     andValue:disease.categoryId
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:DI_SUBCATEGORYID                         
                                                                     andValue:disease.subcategoryId
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:DI_NAME
                                                                     andValue:disease.name
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:DI_OVERVIEW                         
                                                                     andValue:disease.overview
                                                                   andReplace:YES] autorelease]];
    
    SimpleDBPutAttributesRequest *sdbPutRequest = [[SimpleDBPutAttributesRequest alloc] 
                                                   initWithDomainName:DOMAIN_DISEASE 
                                                   andItemName:itemName 
                                                   andAttributes:attributes];
    
    [attributes release];
    [dateFormatter release];
    return [sdbPutRequest autorelease];
}

-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithPresentation:(BDPresentation *)presentation
{
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
    [dateFormatter setTimeStyle:NSDateFormatterFullStyle];
    [dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *itemName = presentation.uuid;
    NSLog(@"ItemName to push:%@", itemName);
    NSMutableArray *attributes = [[NSMutableArray alloc] initWithCapacity:8];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:PR_UUID 
                                                                     andValue:presentation.uuid 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:PR_SCHEMAVERSION 
                                                                     andValue:[NSString stringWithFormat:@"%d", [presentation.schemaVersion intValue]]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:PR_INUSEBY 
                                                                     andValue:(nil == presentation.inUseBy) ? @"" : presentation.inUseBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:PR_CREATEDDATE 
                                                                     andValue:[dateFormatter stringFromDate:presentation.createdDate] 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:PR_CREATEDBY 
                                                                     andValue:(nil == presentation.createdBy) ? @"" : presentation.createdBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:PR_MODIFIEDDATE 
                                                                     andValue:[dateFormatter stringFromDate:presentation.modifiedDate] 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:PR_MODIFIEDBY 
                                                                     andValue:(nil == presentation.modifiedBy) ? @"" : presentation.modifiedBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:PR_DEPRECATED 
                                                                     andValue:[Utility nsNumberBoolToString:presentation.deprecated]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:PR_DISEASEID                         
                                                                     andValue:presentation.diseaseId
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:PR_DISPLAYORDER                         
                                                                     andValue:[NSString stringWithFormat:@"%d", [presentation.displayOrder intValue]]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:PR_NAME
                                                                     andValue:presentation.name
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:PR_OVERVIEW                         
                                                                     andValue:presentation.overview
                                                                   andReplace:YES] autorelease]];
    
    SimpleDBPutAttributesRequest *sdbPutRequest = [[SimpleDBPutAttributesRequest alloc] 
                                                   initWithDomainName:DOMAIN_PRESENTATION
                                                   andItemName:itemName 
                                                   andAttributes:attributes];
    
    [attributes release];
    [dateFormatter release];
    return [sdbPutRequest autorelease];
}

-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithTherapyGroup:(BDTherapyGroup *)therapyGroup
{
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
    [dateFormatter setTimeStyle:NSDateFormatterFullStyle];
    [dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *itemName = therapyGroup.uuid;
    NSLog(@"ItemName to push:%@", itemName);
    NSMutableArray *attributes = [[NSMutableArray alloc] initWithCapacity:8];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TG_UUID 
                                                                     andValue:therapyGroup.uuid 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TG_SCHEMAVERSION 
                                                                     andValue:[NSString stringWithFormat:@"%d", [therapyGroup.schemaVersion intValue]]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TG_INUSEBY 
                                                                     andValue:(nil == therapyGroup.inUseBy) ? @"" : therapyGroup.inUseBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TG_CREATEDDATE 
                                                                     andValue:[dateFormatter stringFromDate:therapyGroup.createdDate] 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TG_CREATEDBY 
                                                                     andValue:(nil == therapyGroup.createdBy) ? @"" : therapyGroup.createdBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TG_MODIFIEDDATE 
                                                                     andValue:[dateFormatter stringFromDate:therapyGroup.modifiedDate] 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TG_MODIFIEDBY 
                                                                     andValue:(nil == therapyGroup.modifiedBy) ? @"" : therapyGroup.modifiedBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TG_DEPRECATED 
                                                                     andValue:[Utility nsNumberBoolToString:therapyGroup.deprecated]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TG_PATHOGENGROUPID                         
                                                                     andValue:therapyGroup.pathogenGroupId
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TG_DISPLAYORDER                         
                                                                     andValue:[NSString stringWithFormat:@"%d", [therapyGroup.displayOrder intValue]]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TG_THERAPYNOTE                         
                                                                     andValue:therapyGroup.therapyNote 
                                                                   andReplace:YES] autorelease]];
    
    SimpleDBPutAttributesRequest *sdbPutRequest = [[SimpleDBPutAttributesRequest alloc] 
                                                   initWithDomainName:DOMAIN_THERAPYGROUP
                                                   andItemName:itemName 
                                                   andAttributes:attributes];
    
    [attributes release];
    [dateFormatter release];
    return [sdbPutRequest autorelease];
}

-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithTherapy:(BDTherapy *)therapy
{
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
    [dateFormatter setTimeStyle:NSDateFormatterFullStyle];
    [dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *itemName = therapy.uuid;
    NSLog(@"ItemName to push:%@", itemName);
    NSMutableArray *attributes = [[NSMutableArray alloc] initWithCapacity:8];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_UUID 
                                                                     andValue:therapy.uuid 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_SCHEMAVERSION 
                                                                     andValue:[NSString stringWithFormat:@"%d", [therapy.schemaVersion intValue]]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_INUSEBY 
                                                                     andValue:(nil == therapy.inUseBy) ? @"" : therapy.inUseBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_CREATEDDATE 
                                                                     andValue:[dateFormatter stringFromDate:therapy.createdDate] 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_CREATEDBY 
                                                                     andValue:(nil == therapy.createdBy) ? @"" : therapy.createdBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_MODIFIEDDATE 
                                                                     andValue:[dateFormatter stringFromDate:therapy.modifiedDate] 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_MODIFIEDBY 
                                                                     andValue:(nil == therapy.modifiedBy) ? @"" : therapy.modifiedBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_DEPRECATED 
                                                                     andValue:[Utility nsNumberBoolToString:therapy.deprecated]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_THERAPYGROUPID                         
                                                                     andValue:therapy.therapyGroupId
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_THERAPYGROUPJOINTYPE                         
                                                                     andValue:[NSString stringWithFormat:@"%d", [therapy.therapyGroupJoinType intValue]]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_DISPLAYORDER                            
                                                                     andValue:[NSString stringWithFormat:@"%d", [therapy.displayOrder intValue]]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_DOSAGE                         
                                                                     andValue:therapy.dosage 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_DURATION                         
                                                                     andValue:therapy.duration 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_NAME                         
                                                                     andValue:therapy.name 
                                                                   andReplace:YES] autorelease]];
    
    SimpleDBPutAttributesRequest *sdbPutRequest = [[SimpleDBPutAttributesRequest alloc] 
                                                   initWithDomainName:DOMAIN_THERAPY
                                                   andItemName:itemName 
                                                   andAttributes:attributes];
    
    [attributes release];
    [dateFormatter release];
    return [sdbPutRequest autorelease];
}

-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithLinkedNote:(BDLinkedNote *)linkedNote
{

    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *itemName = linkedNote.uuid;
    NSLog(@"ItemName to push:%@", itemName);
    NSMutableArray *attributes = [[NSMutableArray alloc] initWithCapacity:8];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_UUID 
                                                                    andValue:linkedNote.uuid 
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_SCHEMAVERSION 
                                                                    andValue:[NSString stringWithFormat:@"%d", [linkedNote.schemaVersion intValue]]
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_STORAGEKEY 
                                                                     andValue:(nil == linkedNote.storageKey) ? @"" : linkedNote.storageKey
                                                                   andReplace:YES] autorelease]];

    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_INUSEBY 
                                                                     andValue:(nil == linkedNote.inUseBy) ? @"" : linkedNote.inUseBy
                                                                   andReplace:YES] autorelease]];

    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_CREATEDDATE 
                                                                    andValue:[dateFormatter stringFromDate:linkedNote.createdDate] 
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_CREATEDBY 
                                                                     andValue:(nil == linkedNote.createdBy) ? @"" : linkedNote.createdBy
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_MODIFIEDDATE 
                                                                    andValue:[dateFormatter stringFromDate:linkedNote.modifiedDate] 
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_MODIFIEDBY 
                                                                     andValue:(nil == linkedNote.modifiedBy) ? @"" : linkedNote.modifiedBy
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_DEPRECATED 
                                                                    andValue:[Utility nsNumberBoolToString:linkedNote.deprecated]
                                                                  andReplace:YES] autorelease]];
    
    SimpleDBPutAttributesRequest *sdbPutRequest = [[SimpleDBPutAttributesRequest alloc] 
                                                   initWithDomainName:DOMAIN_LINKEDNOTE 
                                                   andItemName:itemName 
                                                   andAttributes:attributes];
    
    [attributes release];
    [dateFormatter release];
    return [sdbPutRequest autorelease];
}


#pragma mark - Entity Load from SimpleDB

//// MAKE THIS 'GENERIC' FOR ALL ENTITIES>>>

-(NSString *)loadEntityWithItemName:(NSString *)theItemName forDomain:(NSString *)theDomain
{
    NSString * documentUUID = nil;
    @try 
    {
        SimpleDBGetAttributesRequest  *request = [[[SimpleDBGetAttributesRequest alloc] initWithDomainName:DOMAIN_LINKEDNOTE 
                                                                                           andItemName:theItemName] autorelease];
        
        SimpleDBGetAttributesResponse *response = [[RepositoryConstants sdb] getAttributes:request];
        
        NSMutableDictionary *attributeDictionary = [[NSMutableDictionary alloc] initWithCapacity:[response.attributes count]];
        
        for (SimpleDBAttribute *attr in response.attributes) 
        {
            [attributeDictionary setObject:(nil == attr.value) ? nil : attr.value
                                    forKey:attr.name];
        }
        
        documentUUID = [BDLinkedNote loadWithAttributes:attributeDictionary withOverwriteNewerFlag:NO];        
    }
    @catch (AmazonServiceException *exception) {
        NSLog(@"Exception = %@", exception);
    }

    return documentUUID;
}

#pragma mark - Generic private methods

-(NSString *)getSimpleSelectExpressionForDomain:(NSString *)theDomain {
    NSString *returnString = [NSString stringWithFormat:@"select itemName() from %@", theDomain];
    return returnString;
}

-(NSString *)getSelectExpressionForDomain:(NSString *)theDomain withDateItem:(NSString *)theDateItem beforeDate:(NSString *) theCompareDate {
    NSString *returnString = [NSString stringWithFormat:@"select itemName() from %@ where %@ > '%@'", theDomain, theDateItem, theCompareDate];
    return returnString;
}
@end
