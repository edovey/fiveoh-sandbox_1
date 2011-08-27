//
//  EditorDocument.m
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-18.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "EditorDocument.h"
#import "NSString+UUID.h"
#import "QueueEntry.h"

@interface EditorDocument()
-(NSString *)generateStorageKey;
@end


@implementation EditorDocument

@dynamic uuid;
@dynamic createdDate;
@dynamic createdBy;
@dynamic modifiedDate;
@dynamic modifiedBy;
@dynamic inUseBy;
@dynamic documentText;
@dynamic storageKey;
@dynamic deprecated;
@dynamic schemaVersion;

+(NSString *)create
{
    NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
    NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_EDITORDOCUMENT inManagedObjectContext:moc];

	EditorDocument *document = [[EditorDocument alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];

    document.uuid = [NSString UUIDCreate];
    document.createdDate = [NSDate date];
    document.createdBy = [[UIDevice currentDevice] uniqueIdentifier];
    document.modifiedDate = document.createdDate;
    document.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    document.inUseBy = [[UIDevice currentDevice] uniqueIdentifier];
    document.schemaVersion = [NSNumber numberWithInt:[SCHEMAVERSION_EDITORDOCUMENT intValue]];
    document.deprecated = [NSNumber numberWithBool:NO];
    
    document.storageKey = [NSString stringWithFormat:@"bd~%@.txt", document.uuid];
    
    [QueueEntry createWithObjectUuid:document.uuid 
                      withEntityName:ENTITYNAME_EDITORDOCUMENT 
                          withAction:CREATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
    NSString *uuid = document.uuid;
    
    return uuid;
}

+(EditorDocument *)retrieveWithUUID:(NSString *)theUUID
{
    return (EditorDocument *)[[DataController sharedInstance] retrieveManagedObject:ENTITYNAME_EDITORDOCUMENT 
                                                                               uuid:theUUID 
                                                                          targetMOC:nil];
}

//Returns the uuid of the object
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer
{
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];

    NSString *uuid = [theAttributeDictionary valueForKey:ED_UUID];
    NSDate *modifedDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:ED_MODIFIEDDATE]];
    
    EditorDocument *document = [EditorDocument retrieveWithUUID:uuid];
    if(nil == document)
    {
        NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
        NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_EDITORDOCUMENT 
                                                  inManagedObjectContext:moc];
        
        document = [[EditorDocument alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
        document.uuid = uuid;
        
    }
    
    NSLog(@"Local Document Modified Date:%@", [dateFormatter stringFromDate:document.modifiedDate]);
    NSLog(@"Repository Document Modified Date:%@", [dateFormatter stringFromDate:modifedDate]);
    
    if ( (nil == document.modifiedDate) 
        || ( ([document.modifiedDate compare:modifedDate] == NSOrderedAscending) || (overwriteNewer) ) )
    {
        int schemaVersion = [[theAttributeDictionary valueForKey:ED_SCHEMAVERSION] intValue];
        
        document.schemaVersion = [NSNumber numberWithInt:schemaVersion];
        
        switch (schemaVersion)
        {
            case 1:
            default:
            {
                document.createdBy = [theAttributeDictionary valueForKey:ED_CREATEDBY];
                document.createdDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:ED_CREATEDDATE]];
                document.modifiedBy  = [theAttributeDictionary valueForKey:ED_MODIFIEDBY];
                document.modifiedDate = modifedDate;
                document.inUseBy = [theAttributeDictionary valueForKey:ED_INUSEBY];
                document.storageKey = [theAttributeDictionary valueForKey:ED_STORAGEKEY];   
                document.deprecated = [NSNumber numberWithBool:[[theAttributeDictionary valueForKey:ED_DEPRECATED] boolValue]];
            }
                break;
        }
    }
    else
    {
        NSLog(@"*** Attempted to load a version older than the current for %@", uuid);
        uuid = nil;
    }
    
    [[DataController sharedInstance] saveContext];
    
    return uuid;
}

-(NSString *)generateStorageKey
{
    return [NSString stringWithFormat:@"bd~%@.txt", self.uuid];
}

-(void)commitChanges
{
    self.inUseBy = @""; //Review when this gets toggled. Perhaps on push
    self.modifiedDate = [NSDate date];
    self.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    
    if ( (nil == self.storageKey) || ([self.storageKey length] == 0) )
    {
        self.storageKey = [self generateStorageKey];
    }
    
    
    
    [QueueEntry createWithObjectUuid:self.uuid 
                      withEntityName:ENTITYNAME_EDITORDOCUMENT 
                          withAction:UPDATE_QueueEntryActionType 
                            withSave:NO];

    [[DataController sharedInstance] saveContext];
}


@end
