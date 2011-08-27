//
//  RepositoryHandler.m
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-24.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "RepositoryHandler.h"
#import "RepositoryConstants.h"
#import "QueueEntry.h"
#import "EditorDocument.h"

#import <AWSiOSSDK/SimpleDB/AmazonSimpleDBClient.h>
#import "SdbRequestDelegate.h"

#import "EditorDocument.h"

@interface RepositoryHandler()
{
@private
//    SdbRequestDelegate              *sdbDelegate;
//    SimpleDBPutAttributesRequest    *sdbPutRequest;
}

-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithEditorDocument:(EditorDocument *)editorDocument;
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
    NSString *selectExpression = [NSString stringWithFormat:@"select itemName() from `%@`", DOMAIN_EDITORDOCUMENT];
    
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
        selectExpression = [NSString stringWithFormat:@"select itemName() from %@", DOMAIN_EDITORDOCUMENT];
    }
    else
    {
        selectExpression = [NSString stringWithFormat:@"select itemName() from %@ where %@ > '%@'", DOMAIN_EDITORDOCUMENT, ED_MODIFIEDDATE, refreshDateString];
    }
    @try 
    {
        SimpleDBSelectRequest  *selectRequest  = [[[SimpleDBSelectRequest alloc] initWithSelectExpression:selectExpression] autorelease];
        SimpleDBSelectResponse *selectResponse = [[RepositoryConstants sdb] select:selectRequest];
        
        NSLog(@"Number of items to load:%d", [selectResponse.items count]);
        
        for (SimpleDBItem *item in selectResponse.items) 
        {            
            EditorDocument *document = [EditorDocument retrieveWithUUID:[handler loadEditorDocumentwithItemName:item.name]];
            
            if ((nil != document) && (nil != document.storageKey) && ([document.storageKey length] > 0))
            {
                NSLog(@"Retrieving S3 Doc:%@", document.storageKey);
                
                @try 
                {
                    S3GetObjectRequest  *s3ObjectRequest  = [[S3GetObjectRequest alloc] initWithKey:document.storageKey 
                                                                                           withBucket:BUCKET_EDITORDOCUMENT];
                    
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
                    
//                    [s3ObjectResponse release];
//                    [s3ObjectRequest release];
                }
                @catch (AmazonServiceException *exception) 
                {
                    NSLog(@"Exception = %@", exception);
                }
            }
        }
        
//        [selectRequest release];
//        [selectResponse release];

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
        QueueEntry *queueEntry = [queueEntries objectAtIndex:0];
        
        if ([queueEntry.objectEntityName isEqualToString:ENTITYNAME_EDITORDOCUMENT]) 
        {
            EditorDocument *document = [EditorDocument retrieveWithUUID:queueEntry.objectUuid];
            
            document.inUseBy = @"";
            
            SimpleDBPutAttributesRequest *sdbPutRequest = [self sdbPutAttributeRequestWithEditorDocument:document];
            [[RepositoryConstants sdb] putAttributes:sdbPutRequest];
            
            // Put the file as an object in the bucket.

            S3PutObjectRequest *putObjectRequest = [[S3PutObjectRequest alloc] initWithKey:document.storageKey 
                                                                                  inBucket:BUCKET_EDITORDOCUMENT];
            putObjectRequest.contentType = @"text/plain";
//            NSString *textToEncode = ([document.documentText length] == 0) ? @"<body></body>" : document.documentText;
            
            putObjectRequest.data = [document.documentText dataUsingEncoding:NSUTF8StringEncoding];
            
            [[RepositoryConstants s3] putObject:putObjectRequest];
            
            processCount++;
            
            [[[DataController sharedInstance] managedObjectContext] deleteObject:queueEntry];
            [queueEntries removeObjectAtIndex:0];
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

-(SimpleDBPutAttributesRequest *)sdbPutAttributeRequestWithEditorDocument:(EditorDocument *)editorDocument
{

    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *itemName = editorDocument.uuid;
    NSLog(@"ItemName to push:%@", itemName);
    NSMutableArray *attributes = [[NSMutableArray alloc] initWithCapacity:8];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:ED_UUID 
                                                                    andValue:editorDocument.uuid 
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:ED_SCHEMAVERSION 
                                                                    andValue:[NSString stringWithFormat:@"%d", [editorDocument.schemaVersion intValue]]
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:ED_STORAGEKEY 
                                                                     andValue:(nil == editorDocument.storageKey) ? @"" : editorDocument.storageKey
                                                                   andReplace:YES] autorelease]];

    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:ED_INUSEBY 
                                                                     andValue:(nil == editorDocument.inUseBy) ? @"" : editorDocument.inUseBy
                                                                   andReplace:YES] autorelease]];

    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:ED_CREATEDDATE 
                                                                    andValue:[dateFormatter stringFromDate:editorDocument.createdDate] 
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:ED_CREATEDBY 
                                                                     andValue:(nil == editorDocument.createdBy) ? @"" : editorDocument.createdBy
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:ED_MODIFIEDDATE 
                                                                    andValue:[dateFormatter stringFromDate:editorDocument.modifiedDate] 
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:ED_MODIFIEDBY 
                                                                     andValue:(nil == editorDocument.modifiedBy) ? @"" : editorDocument.modifiedBy
                                                                  andReplace:YES] autorelease]];
    
    [attributes addObject:[[[SimpleDBReplaceableAttribute alloc] initWithName:ED_DEPRECATED 
                                                                    andValue:[Utility nsNumberBoolToString:editorDocument.deprecated]
                                                                  andReplace:YES] autorelease]];
    
    SimpleDBPutAttributesRequest *sdbPutRequest = [[SimpleDBPutAttributesRequest alloc] 
                                                   initWithDomainName:DOMAIN_EDITORDOCUMENT 
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
        SimpleDBGetAttributesRequest  *request = [[[SimpleDBGetAttributesRequest alloc] initWithDomainName:DOMAIN_EDITORDOCUMENT 
                                                                                           andItemName:theItemName] autorelease];
        
        SimpleDBGetAttributesResponse *response = [[RepositoryConstants sdb] getAttributes:request];
        
        NSMutableDictionary *attributeDictionary = [[NSMutableDictionary alloc] initWithCapacity:[response.attributes count]];
        
        for (SimpleDBAttribute *attr in response.attributes) 
        {
            [attributeDictionary setObject:(nil == attr.value) ? [NSNull null] : attr.value
                                    forKey:attr.name];
        }
        
        documentUUID = [EditorDocument loadWithAttributes:attributeDictionary withOverwriteNewerFlag:NO];        
    }
    @catch (AmazonServiceException *exception) {
        NSLog(@"Exception = %@", exception);
    }

    return documentUUID;
}

/*
-(void)dealloc
{
    
    [sdbDelegate dealloc];
    [sdbPutRequest release];
    [super dealloc];
}
*/
@end
