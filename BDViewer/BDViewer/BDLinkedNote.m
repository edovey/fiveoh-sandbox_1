//
//  EditorDocument.m
//  BDViewer
//
//  Created by Karl Schulze on 2011-08-18.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "BDLinkedNote.h"
#import "NSString+UUID.h"
#import "BDQueueEntry.h"

@interface BDLinkedNote()
-(NSString *)generateStorageKey;
@end


@implementation BDLinkedNote

@dynamic uuid;
@dynamic createdDate;
@dynamic createdBy;
@dynamic modifiedDate;
@dynamic modifiedBy;
@dynamic inUseBy;
@dynamic deprecated;
@dynamic schemaVersion;
@dynamic displayOrder;
@dynamic linkedNoteAssociationId;
@dynamic previewText;
@dynamic scopeId;
@dynamic singleUse;
@dynamic storageKey;
@dynamic documentText;

+(NSString *)create
{
    NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
    NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_LINKEDNOTE inManagedObjectContext:moc];

	BDLinkedNote *document = [[BDLinkedNote alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];

    document.uuid = [NSString UUIDCreate];
    document.createdDate = [NSDate date];
    document.createdBy = [[UIDevice currentDevice] uniqueIdentifier];
    document.modifiedDate = document.createdDate;
    document.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    document.inUseBy = nil;
    document.schemaVersion = [NSNumber numberWithInt:[SCHEMAVERSION_LINKEDNOTE intValue]];
    document.deprecated = [NSNumber numberWithBool:NO];
    document.displayOrder = [NSNumber numberWithInt:-1];
    
    document.storageKey = [NSString stringWithFormat:@"bd~%@.txt", document.uuid];
    
    [BDQueueEntry createWithObjectUuid:document.uuid 
                      withEntityName:ENTITYNAME_LINKEDNOTE 
                          withAction:CREATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
    NSString *uuid = document.uuid;
    [document release];
    
    return uuid;
}

+(BDLinkedNote *)retrieveWithUUID:(NSString *)theUUID
{
    return (BDLinkedNote *)[[DataController sharedInstance] retrieveManagedObject:ENTITYNAME_LINKEDNOTE 
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

    NSString *uuid = [theAttributeDictionary valueForKey:LN_UUID];
    NSDate *modifedDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:LN_MODIFIEDDATE]];
    
    BDLinkedNote *document = [BDLinkedNote retrieveWithUUID:uuid];
    if(nil == document)
    {
        NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
        NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_LINKEDNOTE 
                                                  inManagedObjectContext:moc];
        
        document = [[[BDLinkedNote alloc] initWithEntity:entity insertIntoManagedObjectContext:moc] autorelease];
        document.uuid = uuid;
        
    }
    
    NSLog(@"Local Linked Note Modified Date:%@", [dateFormatter stringFromDate:document.modifiedDate]);
    NSLog(@"Repository Linked Note Modified Date:%@", [dateFormatter stringFromDate:modifedDate]);
    
    if ( (nil == document.modifiedDate) 
        || ( ([document.modifiedDate compare:modifedDate] == NSOrderedAscending) || (overwriteNewer) ) )
    {
        int schemaVersion = [[theAttributeDictionary valueForKey:LN_SCHEMAVERSION] intValue];
        
        document.schemaVersion = [NSNumber numberWithInt:schemaVersion];
        
        switch (schemaVersion)
        {
            case 1:
            default:
            {
                document.createdBy = [theAttributeDictionary valueForKey:LN_CREATEDBY];
                document.createdDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:LN_CREATEDDATE]];
                document.modifiedBy  = [theAttributeDictionary valueForKey:LN_MODIFIEDBY];
                document.modifiedDate = modifedDate;
                document.inUseBy = [theAttributeDictionary valueForKey:LN_INUSEBY];
                document.deprecated = [NSNumber numberWithBool:[[theAttributeDictionary valueForKey:LN_DEPRECATED] boolValue]];
                document.displayOrder = [NSNumber numberWithInt:[[theAttributeDictionary valueForKey:LN_DISPLAYORDER] intValue]];
                document.linkedNoteAssociationId = [theAttributeDictionary valueForKey:LN_LINKEDNOTEASSOCIATIONID];
                document.previewText = [theAttributeDictionary valueForKey:LN_PREVIEWTEXT];
                document.scopeId = [theAttributeDictionary valueForKey:LN_SCOPEID];
                document.singleUse = [NSNumber numberWithBool:[[theAttributeDictionary valueForKey:LN_SINGLEUSE] boolValue] ];
                document.storageKey = [theAttributeDictionary valueForKey:LN_STORAGEKEY];   
                document.documentText = [theAttributeDictionary valueForKey:LN_DOCUMENTTEXT];
            }
                break;
        }
    }
    else
    {
        NSLog(@"*** Attempted to load a version older than the current for %@", uuid);
        uuid = nil;
    }
    [dateFormatter release];
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
    
    
    
    [BDQueueEntry createWithObjectUuid:self.uuid 
                      withEntityName:ENTITYNAME_LINKEDNOTE 
                          withAction:UPDATE_QueueEntryActionType 
                            withSave:NO];

    [[DataController sharedInstance] saveContext];
}


@end
