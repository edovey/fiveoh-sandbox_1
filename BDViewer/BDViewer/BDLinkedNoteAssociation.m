//
//  BDLinkedNoteAssociation.m
//  BDViewer
//
//  Created by Liz Dovey on 11-10-14.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "BDLinkedNoteAssociation.h"
#import "NSString+UUID.h"
#import "BDQueueEntry.h"

@implementation BDLinkedNoteAssociation

@dynamic uuid;
@dynamic createdBy;
@dynamic createdDate;
@dynamic modifiedBy;
@dynamic modifiedDate;
@dynamic schemaVersion;
@dynamic deprecated;
@dynamic displayOrder;
@dynamic linkedNoteId;
@dynamic parentId;
@dynamic parentEntityName;
@dynamic parentEntityPropertyName;
@dynamic linkedNoteType;



+(NSString *)create
{
    NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
    NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_LINKEDNOTEASSOCIATION inManagedObjectContext:moc];
    
	BDLinkedNoteAssociation *lnAssociation = [[BDLinkedNoteAssociation alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
    
    lnAssociation.uuid = [NSString UUIDCreate];
    lnAssociation.createdDate = [NSDate date];
    lnAssociation.createdBy = [[UIDevice currentDevice] uniqueIdentifier];
    lnAssociation.modifiedDate = lnAssociation.createdDate;
    lnAssociation.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    lnAssociation.schemaVersion = [NSNumber numberWithInt:[SCHEMAVERSION_LINKEDNOTEASSOCIATION intValue]];
    lnAssociation.deprecated = [NSNumber numberWithBool:NO];
    lnAssociation.displayOrder = [NSNumber numberWithInt:-1];
    
    [BDQueueEntry createWithObjectUuid:lnAssociation.uuid 
                        withEntityName:ENTITYNAME_LINKEDNOTEASSOCIATION 
                            withAction:CREATE_QueueEntryActionType 
                              withSave:NO];
    
    [[DataController sharedInstance] saveContext];
    NSString *uuid = lnAssociation.uuid;
    [lnAssociation release];
    
    return uuid;
}

+(BDLinkedNoteAssociation *)retrieveWithUUID:(NSString *)theUUID
{
    return (BDLinkedNoteAssociation *)[[DataController sharedInstance] retrieveManagedObject:ENTITYNAME_LINKEDNOTEASSOCIATION 
                                                                             uuid:theUUID 
                                                                        targetMOC:nil];
}

+(NSArray *) retrieveAllWithParentUUID:(NSString *)theUUID
{
    NSArray *entities = [[DataController sharedInstance] retrieveManagedObjectsForValue:ENTITYNAME_LINKEDNOTEASSOCIATION withKey:@"parentId" withValue:theUUID orderedBy:@"displayOrder" withMOC:nil];
    return entities;
}



//Returns the uuid of the object
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer
{
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *uuid = [theAttributeDictionary valueForKey:LA_UUID];
    NSDate *modifedDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:LA_MODIFIEDDATE]];
    
    BDLinkedNoteAssociation *lnAssociation = [BDLinkedNoteAssociation retrieveWithUUID:uuid];
    if(nil == lnAssociation)
    {
        NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
        NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_LINKEDNOTEASSOCIATION 
                                                  inManagedObjectContext:moc];
        
        lnAssociation = [[[BDLinkedNoteAssociation alloc] initWithEntity:entity insertIntoManagedObjectContext:moc] autorelease];
        lnAssociation.uuid = uuid;
        
    }
    
    NSLog(@"Local Linked Note Association Modified Date:%@", [dateFormatter stringFromDate:lnAssociation.modifiedDate]);
    NSLog(@"Repository Linked Note Association Modified Date:%@", [dateFormatter stringFromDate:modifedDate]);
    
    if ( (nil == lnAssociation.modifiedDate) 
        || ( ([lnAssociation.modifiedDate compare:modifedDate] == NSOrderedAscending) || (overwriteNewer) ) )
    {
        int schemaVersion = [[theAttributeDictionary valueForKey:LA_SCHEMAVERSION] intValue];
        
        lnAssociation.schemaVersion = [NSNumber numberWithInt:schemaVersion];
        
        switch (schemaVersion)
        {
            case 1:
            default:
            {
                lnAssociation.createdBy = [theAttributeDictionary valueForKey:LA_CREATEDBY];
                lnAssociation.createdDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:LA_CREATEDDATE]];
                lnAssociation.modifiedBy  = [theAttributeDictionary valueForKey:LA_MODIFIEDBY];
                lnAssociation.modifiedDate = modifedDate;
                lnAssociation.deprecated = [NSNumber numberWithBool:[[theAttributeDictionary valueForKey:LA_DEPRECATED] boolValue]];
                lnAssociation.displayOrder = [NSNumber numberWithInt:[[theAttributeDictionary valueForKey:LA_DISPLAYORDER] intValue]];
                lnAssociation.linkedNoteId = [theAttributeDictionary valueForKey:LA_LINKEDNOTEID];
                lnAssociation.parentId = [theAttributeDictionary valueForKey:LA_PARENTID];
                lnAssociation.parentEntityName = [theAttributeDictionary valueForKey:LA_PARENTENTITYNAME];
                lnAssociation.parentEntityPropertyName = [theAttributeDictionary valueForKey:LA_PARENTENTITYPROPERTYNAME];
                lnAssociation.linkedNoteType = [NSNumber numberWithInt:[[theAttributeDictionary valueForKey:LA_LINKEDNOTETYPE] intValue] ];   
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

-(void)commitChanges
{
    self.modifiedDate = [NSDate date];
    self.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    
    
    [BDQueueEntry createWithObjectUuid:self.uuid 
                        withEntityName:ENTITYNAME_LINKEDNOTEASSOCIATION 
                            withAction:UPDATE_QueueEntryActionType 
                              withSave:NO];
    
    [[DataController sharedInstance] saveContext];
}


@end
