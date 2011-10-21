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
#import "BDChapter.h"
#import "BDSubcategory.h"
#import "BDDisease.h"
#import "BDPresentation.h"
#import "BDTherapyGroup.h"
#import "BDTherapy.h"
#import "BDLinkedNoteAssociation.h"  
#import "BDPathogen.h"
#import "BDPathogenGroup.h"

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
-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithLinkedNoteAssociation:(BDLinkedNoteAssociation *)linkedNoteAssociation;
-(int)pushQueuedEntries;
-(NSString *)loadEntityWithAwsSimpleDbItemName:(NSString *)theItemName forAwsSimpleDbDomain:(NSString *)theDomain;
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

/*
+(void)pullAll
{
    RepositoryHandler *handler = [[RepositoryHandler alloc] init];
    
    NSArray *entityArray = [[NSArray alloc] initWithObjects:
                            DOMAIN_SECTION, 
                            DOMAIN_CATEGORY, 
                            DOMAIN_SUBCATEGORY, 
                            DOMAIN_DISEASE, 
                            DOMAIN_PATHOGEN,
                            DOMAIN_PATHOGENGROUP,
                            DOMAIN_PRESENTATION, 
                            DOMAIN_THERAPYGROUP, 
                            DOMAIN_THERAPY, 
                            DOMAIN_LINKEDNOTE, 
                            DOMAIN_LINKEDNOTEASSOCIATION, 
                            nil];
    
    for (int i = 0; i < [entityArray count]; i++)
    {
        NSString *selectExpression = [handler getSimpleSelectExpressionForDomain:[entityArray objectAtIndex:i]];
        @try 
        {
            SimpleDBSelectRequest  *selectRequest  = [[[SimpleDBSelectRequest alloc] initWithSelectExpression:selectExpression] autorelease];
            SimpleDBSelectResponse *selectResponse = [[RepositoryConstants sdb] select:selectRequest];
            
            for (SimpleDBItem *item in selectResponse.items) 
            {
                NSString *loadedUUID = [handler loadEntityWithAwsSimpleDbItemName:item.name forAwsSimpleDbDomain:[entityArray objectAtIndex:i]];
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
*/

+(void)pullSince:(NSDate *)theLastSyncDate
{
    RepositoryHandler *handler = [[RepositoryHandler alloc] init];

    NSArray *entityArray = [[NSArray alloc] initWithObjects:
                            DOMAIN_CHAPTER,
                            DOMAIN_SECTION, 
                            DOMAIN_CATEGORY, 
                            DOMAIN_SUBCATEGORY, 
                            DOMAIN_DISEASE, 
                            DOMAIN_PATHOGEN,
                            DOMAIN_PATHOGENGROUP,
                            DOMAIN_PRESENTATION, 
                            DOMAIN_THERAPYGROUP, 
                            DOMAIN_THERAPY, 
                            DOMAIN_LINKEDNOTE, 
                            DOMAIN_LINKEDNOTEASSOCIATION, 
                            nil];
    
    NSArray *entityDateArray = [[NSArray alloc] initWithObjects: 
                                CH_MODIFIEDDATE,
                                SN_MODIFIEDDATE, 
                                CT_MODIFIEDDATE, 
                                SC_MODIFIEDDATE, 
                                DI_MODIFIEDDATE, 
                                PA_MODIFIEDDATE,
                                PG_MODIFIEDDATE,
                                PR_MODIFIEDDATE, 
                                TG_MODIFIEDDATE, 
                                TH_MODIFIEDDATE, 
                                LN_MODIFIEDDATE, 
                                LA_MODIFIEDDATE, 
                                nil]; 
    
    NSArray *entityBucketArray = [[NSArray alloc] initWithObjects: 
                                  BUCKET_CHAPTER,
                                  BUCKET_SECTION, 
                                  BUCKET_CATEGORY, 
                                  BUCKET_SUBCATEGORY, 
                                  BUCKET_DISEASE, 
                                  BUCKET_PATHOGEN,
                                  BUCKET_PATHOGENGROUP,
                                  BUCKET_PRESENTATION, 
                                  BUCKET_THERAPYGROUP, 
                                  BUCKET_THERAPY, 
                                  BUCKET_LINKEDNOTE, 
                                  BUCKET_LINKEDNOTEASSOCIATION, 
                                  nil];

    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];

    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
//    NSObject *refreshDateInfo = [defaults objectForKey:@"RepositoryRefreshDate"]; // Get the datetime of the last refresh
    
    
    NSString *refreshDateString = nil;
    
    if(nil != theLastSyncDate)
    {
        refreshDateString = [dateFormatter stringFromDate:theLastSyncDate];
    }
        
    NSLog(@"Last Refresh: %@", refreshDateString);
    
    NSString *selectExpression;
    
    for (int i = 0; i < [entityArray count]; i++)
    {
        
        if(nil == refreshDateString)
        {
            selectExpression = [handler getSimpleSelectExpressionForDomain: [entityArray objectAtIndex:i]];
        }
        else
        {
            selectExpression = [handler getSelectExpressionForDomain:[entityArray objectAtIndex:i] withDateItem:[entityDateArray objectAtIndex:i] beforeDate: refreshDateString];
        }
        
        
        @try 
        {
            SimpleDBSelectRequest  *selectRequest  = [[[SimpleDBSelectRequest alloc] initWithSelectExpression:selectExpression] autorelease];
            SimpleDBSelectResponse *selectResponse = [[RepositoryConstants sdb] select:selectRequest];
            
            NSLog(@"Number of items to load:%d", [selectResponse.items count]);
            
            for (SimpleDBItem *item in selectResponse.items) 
            {   
                NSString *loadedUuid = [handler loadEntityWithAwsSimpleDbItemName:item.name forAwsSimpleDbDomain:[entityArray objectAtIndex:i]];
                
                if(nil != loadedUuid)
                {
                    if([entityArray objectAtIndex:i]  == DOMAIN_LINKEDNOTE) 
                    {
                        //Load document text from S3
                        
                        BDLinkedNote *document = [BDLinkedNote retrieveWithUUID:loadedUuid];
                        if ((nil != document) && (nil != document.storageKey) && ([document.storageKey length] > 0))
                        {
                            NSLog(@"Retrieving S3 Doc:%@", document.storageKey);
                            
                            @try 
                            {
                                S3GetObjectRequest  *s3ObjectRequest  = [[S3GetObjectRequest alloc] initWithKey:document.storageKey 
                                                                                                     withBucket:[entityBucketArray objectAtIndex: i]];
                                
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

            }
        }
        @catch (AmazonServiceException *exception) 
        {
            NSLog(@"Exception = %@", exception);
        }
    }
    
    [handler release];

    [defaults setObject:[NSDate date] forKey:@"RepositoryRefreshDate"];
    
    [dateFormatter release];
}

-(int)pushQueuedEntries
{
    NSMutableArray *queueEntries = [[DataController sharedInstance] allInstancesOf:@"QueueEntry" 
                                                                         orderedBy:nil 
                                                                          loadData:NO 
      
                                                                         targetMOC:nil];
    
//    sdbDelegate = [[SdbRequestDelegate alloc] init];
//    NSArray *entityNameArray = [[NSArray alloc] initWithObjects:ENTITYNAME_SECTION, ENTITYNAME_CATEGORY, ENTITYNAME_SUBCATEGORY, ENTITYNAME_DISEASE, ENTITYNAME_PRESENTATION, ENTITYNAME_THERAPYGROUP, ENTITYNAME_THERAPY, ENTITYNAME_LINKEDNOTE, ENTITYNAME_LINKEDNOTEASSOCIATION, nil];
//
//   
//    NSArray *entityBucketArray = [[NSArray alloc] initWithObjects: BUCKET_SECTION, BUCKET_CATEGORY, BUCKET_SUBCATEGORY, BUCKET_DISEASE, BUCKET_PRESENTATION, BUCKET_THERAPYGROUP, BUCKET_THERAPY, BUCKET_LINKEDNOTE, BUCKET_LINKEDNOTEASSOCIATION, nil];


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
                                                                     andValue:(nil == section.name) ? @"" : section.name
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
                                                                     andValue:(nil == category.sectionId) ? @"" : category.sectionId
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:CT_NAME
                                                                     andValue:(nil == category.name) ? @"" : category.name
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
                                                                     andValue:(nil == subcategory.categoryId) ? @"" : subcategory.categoryId
                                                                   andReplace:YES] autorelease]];
     
     [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:SC_NAME
                                                                      andValue:(nil == subcategory.name) ? @"" : subcategory.name
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
                                                                     andValue:(nil == disease.categoryId) ? @"" : disease.categoryId
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:DI_SUBCATEGORYID                         
                                                                     andValue:(nil == disease.subcategoryId) ? @"" : disease.subcategoryId
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:DI_NAME
                                                                     andValue:(nil == disease.name) ? @"" : disease.name
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
                                                                     andValue:(nil == presentation.diseaseId) ? @"" : presentation.diseaseId
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:PR_DISPLAYORDER                         
                                                                     andValue:[NSString stringWithFormat:@"%d", [presentation.displayOrder intValue]]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:PR_NAME
                                                                     andValue:presentation.name
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
                                                                     andValue:(nil == therapyGroup.pathogenGroupId) ? @"" : therapyGroup.pathogenGroupId
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TG_DISPLAYORDER                         
                                                                     andValue:[NSString stringWithFormat:@"%d", [therapyGroup.displayOrder intValue]]
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
                                                                     andValue:(nil == therapy.therapyGroupId) ? @"" : therapy.therapyGroupId
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_THERAPYGROUPJOINTYPE                         
                                                                     andValue:[NSString stringWithFormat:@"%d", [therapy.therapyGroupJoinType intValue]]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_DISPLAYORDER                            
                                                                     andValue:[NSString stringWithFormat:@"%d", [therapy.displayOrder intValue]]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_DOSAGE                         
                                                                     andValue:(nil == therapy.dosage) ? @"" : therapy.dosage 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_DURATION                         
                                                                     andValue:(nil == therapy.duration) ? @"" : therapy.duration 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:TH_NAME                         
                                                                     andValue:(nil == therapy.name) ? @"" : therapy.name 
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
    
    NSDictionary *attributeDictionary = [linkedNote propertyAttributeDictionary];
    
    NSMutableArray *attributes = [[NSMutableArray alloc] initWithCapacity:[attributeDictionary count]];
    
    for (NSString *attributeKey in attributeDictionary)
    {
        NSString *attributeValue = [attributeDictionary valueForKey:attributeKey];
        [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:attributeKey 
                                                                         andValue:attributeValue 
                                                                       andReplace:YES] autorelease]];
    }

    SimpleDBPutAttributesRequest *sdbPutRequest = [[SimpleDBPutAttributesRequest alloc] 
                                                   initWithDomainName:DOMAIN_LINKEDNOTE 
                                                   andItemName:itemName 
                                                   andAttributes:attributes];
    
    [attributes release];
    [dateFormatter release];
    return [sdbPutRequest autorelease];
}

-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithLinkedNoteAssociation:(BDLinkedNoteAssociation *)linkedNoteAssociation
{
    
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *itemName = linkedNoteAssociation.uuid;
    NSLog(@"ItemName to push:%@", itemName);
    NSMutableArray *attributes = [[NSMutableArray alloc] initWithCapacity:8];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LA_UUID 
                                                                     andValue:linkedNoteAssociation.uuid 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LA_SCHEMAVERSION 
                                                                     andValue:[NSString stringWithFormat:@"%d", [linkedNoteAssociation.schemaVersion intValue]]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LA_CREATEDDATE 
                                                                     andValue:[dateFormatter stringFromDate:linkedNoteAssociation.createdDate] 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LA_CREATEDBY 
                                                                     andValue:(nil == linkedNoteAssociation.createdBy) ? @"" : linkedNoteAssociation.createdBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LA_MODIFIEDDATE 
                                                                     andValue:[dateFormatter stringFromDate:linkedNoteAssociation.modifiedDate] 
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LA_MODIFIEDBY 
                                                                     andValue:(nil == linkedNoteAssociation.modifiedBy) ? @"" : linkedNoteAssociation.modifiedBy
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LA_DEPRECATED 
                                                                     andValue:[Utility nsNumberBoolToString:linkedNoteAssociation.deprecated]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LA_DISPLAYORDER 
                                                                     andValue:[NSString stringWithFormat:@"%d",[linkedNoteAssociation.displayOrder intValue]]
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LA_LINKEDNOTEID
                                                                     andValue:(nil == linkedNoteAssociation.linkedNoteId) ? @"" : linkedNoteAssociation.linkedNoteId
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LA_PARENTID
                                                                     andValue:(nil == linkedNoteAssociation.parentId) ? @"" : linkedNoteAssociation.parentId
                                                                   andReplace:YES] autorelease]];

    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LA_PARENTENTITYNAME
                                                                     andValue:(nil == linkedNoteAssociation.parentEntityName) ? @"" : linkedNoteAssociation.parentEntityName
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LA_PARENTENTITYPROPERTYNAME
                                                                     andValue:(nil == linkedNoteAssociation.parentEntityPropertyName) ? @"" : linkedNoteAssociation.parentEntityPropertyName
                                                                   andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LA_LINKEDNOTETYPE
                                                                     andValue:[NSString stringWithFormat:@"%d", [linkedNoteAssociation.linkedNoteType intValue]]
                                                                   andReplace:YES] autorelease]];
    

    
    
    SimpleDBPutAttributesRequest *sdbPutRequest = [[SimpleDBPutAttributesRequest alloc] 
                                                   initWithDomainName:DOMAIN_LINKEDNOTEASSOCIATION 
                                                   andItemName:itemName 
                                                   andAttributes:attributes];
    
    [attributes release];
    [dateFormatter release];
    return [sdbPutRequest autorelease];
}


#pragma mark - Generic private methods

-(NSString *)loadEntityWithAwsSimpleDbItemName:(NSString *)theItemName forAwsSimpleDbDomain:(NSString *)theDomain
{
    NSString * documentUUID = nil;
    @try 
    {
        SimpleDBGetAttributesRequest  *request = [[[SimpleDBGetAttributesRequest alloc] initWithDomainName:theDomain 
                                                                                           andItemName:theItemName] autorelease];
        
        SimpleDBGetAttributesResponse *response = [[RepositoryConstants sdb] getAttributes:request];
        
        NSMutableDictionary *attributeDictionary = [[NSMutableDictionary alloc] initWithCapacity:[response.attributes count]];
        
        for (SimpleDBAttribute *attr in response.attributes) 
        {
            [attributeDictionary setObject:(nil == attr.value) ? nil : attr.value
                                    forKey:attr.name];
        }
        
        if([theDomain isEqualToString:DOMAIN_CATEGORY]) { documentUUID = [BDCategory loadWithAttributes:attributeDictionary withOverwriteNewerFlag:NO]; }
        else if([theDomain isEqualToString:DOMAIN_CHAPTER]) { documentUUID = [BDChapter loadWithAttributes:attributeDictionary withOverwriteNewerFlag:NO]; }
        else if([theDomain isEqualToString:DOMAIN_DISEASE]) { documentUUID = [BDDisease loadWithAttributes:attributeDictionary withOverwriteNewerFlag:NO]; }
        else if([theDomain isEqualToString:DOMAIN_LINKEDNOTE]) { documentUUID = [BDLinkedNote loadWithAttributes:attributeDictionary withOverwriteNewerFlag:NO]; }
        else if([theDomain isEqualToString:DOMAIN_LINKEDNOTEASSOCIATION]) { documentUUID = [BDLinkedNoteAssociation loadWithAttributes:attributeDictionary withOverwriteNewerFlag:NO]; }
        else if([theDomain isEqualToString:DOMAIN_PATHOGEN]) { documentUUID = [BDPathogen loadWithAttributes:attributeDictionary withOverwriteNewerFlag:NO]; }
        else if([theDomain isEqualToString:DOMAIN_PATHOGENGROUP]) { documentUUID = [BDPathogenGroup loadWithAttributes:attributeDictionary withOverwriteNewerFlag:NO]; }
        else if([theDomain isEqualToString:DOMAIN_PRESENTATION]) { documentUUID = [BDPresentation loadWithAttributes:attributeDictionary withOverwriteNewerFlag:NO]; }
        else if([theDomain isEqualToString:DOMAIN_SECTION]) { documentUUID = [BDSection loadWithAttributes:attributeDictionary withOverwriteNewerFlag:NO]; }
        else if([theDomain isEqualToString:DOMAIN_SUBCATEGORY]) { documentUUID = [BDSubcategory loadWithAttributes:attributeDictionary withOverwriteNewerFlag:NO]; }
        else if([theDomain isEqualToString:DOMAIN_THERAPY]) { documentUUID = [BDTherapy loadWithAttributes:attributeDictionary withOverwriteNewerFlag:NO]; }
        else if([theDomain isEqualToString:DOMAIN_THERAPYGROUP]) { documentUUID = [BDTherapyGroup loadWithAttributes:attributeDictionary withOverwriteNewerFlag:NO]; }
        
    }
    @catch (AmazonServiceException *exception) {
        NSLog(@"Exception = %@", exception);
    }

    return documentUUID;
}


-(NSString *)getSimpleSelectExpressionForDomain:(NSString *)theDomain {
    NSString *returnString = [NSString stringWithFormat:@"select itemName() from %@", theDomain];
    return returnString;
}

-(NSString *)getSelectExpressionForDomain:(NSString *)theDomain withDateItem:(NSString *)theDateItem beforeDate:(NSString *) theCompareDate {
    NSString *returnString = [NSString stringWithFormat:@"select itemName() from %@ where %@ > '%@'", theDomain, theDateItem, theCompareDate];
    return returnString;
}
@end
