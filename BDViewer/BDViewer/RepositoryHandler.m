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

-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithEditorDocument:(BDLinkedNote *)editorDocument;
-(int)pushQueuedEntries;
-(NSString *)loadEditorDocumentwithItemName:(NSString *)theItemName;


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
    //TODO: overall structure
    
    // EditorDocuments
    NSString *selectExpression = [NSString stringWithFormat:@"select itemName() from `%@`", DOMAIN_LINKEDNOTE];
    
    @try 
    {
        SimpleDBSelectRequest  *selectRequest  = [[[SimpleDBSelectRequest alloc] initWithSelectExpression:selectExpression] autorelease];
        SimpleDBSelectResponse *selectResponse = [[RepositoryConstants sdb] select:selectRequest];
        
        for (SimpleDBItem *item in selectResponse.items) 
        {
            NSString *loadedUUID = [handler loadEditorDocumentwithItemName:item.name];
            NSLog(@"Loaded %@", loadedUUID);
        }
        
    }
    @catch (AmazonServiceException *exception) 
    {
        NSLog(@"Exception = %@", exception);
    }

    [handler release];
    
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
    
    //TODO: overall structure
    
    // EditorDocuments

    if(nil == refreshDateString)
    {
        selectExpression = [NSString stringWithFormat:@"select itemName() from %@", DOMAIN_LINKEDNOTE];
    }
    else
    {
        selectExpression = [NSString stringWithFormat:@"select itemName() from %@ where %@ > '%@'", DOMAIN_LINKEDNOTE, LN_MODIFIEDDATE, refreshDateString];
    }
    @try 
    {
        SimpleDBSelectRequest  *selectRequest  = [[[SimpleDBSelectRequest alloc] initWithSelectExpression:selectExpression] autorelease];
        SimpleDBSelectResponse *selectResponse = [[RepositoryConstants sdb] select:selectRequest];
        
        NSLog(@"Number of items to load:%d", [selectResponse.items count]);
        
        for (SimpleDBItem *item in selectResponse.items) 
        {            
            BDLinkedNote *document = [BDLinkedNote retrieveWithUUID:[handler loadEditorDocumentwithItemName:item.name]];
            
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
            
            SimpleDBPutAttributesRequest *sdbPutRequest = [self sdbPutAttributeRequestWithEditorDocument:document];
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

-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithEditorDocument:(BDLinkedNote *)editorDocument
{

    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *itemName = editorDocument.uuid;
    NSLog(@"ItemName to push:%@", itemName);
    NSMutableArray *attributes = [[NSMutableArray alloc] initWithCapacity:8];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_UUID 
                                                                    andValue:editorDocument.uuid 
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_SCHEMAVERSION 
                                                                    andValue:[NSString stringWithFormat:@"%d", [editorDocument.schemaVersion intValue]]
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_STORAGEKEY 
                                                                     andValue:(nil == editorDocument.storageKey) ? @"" : editorDocument.storageKey
                                                                   andReplace:YES] autorelease]];

    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_INUSEBY 
                                                                     andValue:(nil == editorDocument.inUseBy) ? @"" : editorDocument.inUseBy
                                                                   andReplace:YES] autorelease]];

    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_CREATEDDATE 
                                                                    andValue:[dateFormatter stringFromDate:editorDocument.createdDate] 
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_CREATEDBY 
                                                                     andValue:(nil == editorDocument.createdBy) ? @"" : editorDocument.createdBy
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_MODIFIEDDATE 
                                                                    andValue:[dateFormatter stringFromDate:editorDocument.modifiedDate] 
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_MODIFIEDBY 
                                                                     andValue:(nil == editorDocument.modifiedBy) ? @"" : editorDocument.modifiedBy
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:LN_DEPRECATED 
                                                                    andValue:[Utility nsNumberBoolToString:editorDocument.deprecated]
                                                                  andReplace:YES] autorelease]];
    
    SimpleDBPutAttributesRequest *sdbPutRequest = [[SimpleDBPutAttributesRequest alloc] 
                                                   initWithDomainName:DOMAIN_LINKEDNOTE 
                                                   andItemName:itemName 
                                                   andAttributes:attributes];
    
    [attributes release];
    [dateFormatter release];
    return [sdbPutRequest autorelease];
}

-(NSString *)loadEditorDocumentwithItemName:(NSString *)theItemName
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

@end
