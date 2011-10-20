//
//  BDPathogenGroup.m
//  BDViewer
//
//  Created by Liz Dovey on 11-10-05.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "BDPathogenGroup.h"
#import "NSString+UUID.h"
#import "BDQueueEntry.h"


@implementation BDPathogenGroup
@dynamic uuid;
@dynamic createdBy;
@dynamic createdDate;
@dynamic deprecated;
@dynamic inUseBy;
@dynamic modifiedBy;
@dynamic modifiedDate;
@dynamic schemaVersion;
@dynamic displayOrder;
@dynamic presentationId;

+(NSString *)create
{
    NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
    NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_PATHOGENGROUP inManagedObjectContext:moc];
    
	BDPathogenGroup *pathogenGroup = [[BDPathogenGroup alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
    
    pathogenGroup.uuid = [NSString UUIDCreate];
    pathogenGroup.createdDate = [NSDate date];
    pathogenGroup.createdBy = [[UIDevice currentDevice] uniqueIdentifier];
    pathogenGroup.modifiedDate = pathogenGroup.createdDate;
    pathogenGroup.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    pathogenGroup.inUseBy = nil;
    pathogenGroup.schemaVersion = [NSNumber numberWithInt:[SCHEMAVERSION_PATHOGEN intValue]];
    pathogenGroup.deprecated = [NSNumber numberWithBool:NO];
    pathogenGroup.displayOrder = [NSNumber numberWithInt:-1];
    
    [BDQueueEntry createWithObjectUuid:pathogenGroup.uuid 
                        withEntityName:ENTITYNAME_PATHOGENGROUP 
                            withAction:CREATE_QueueEntryActionType 
                              withSave:NO];
    
    [[DataController sharedInstance] saveContext];
    NSString *uuid = pathogenGroup.uuid;
    [pathogenGroup release];
    
    return uuid;
}

+(BDPathogenGroup *)retrieveWithUUID:(NSString *)theUUID
{
    return (BDPathogenGroup *)[[DataController sharedInstance] retrieveManagedObject:ENTITYNAME_PATHOGENGROUP
                                                                           uuid:theUUID 
                                                                      targetMOC:nil];
}

+(NSArray *) retrieveAllWithParentUUID:(NSString *)theUUID
{
    NSArray *entities = [[DataController sharedInstance] retrieveManagedObjectsForValue:ENTITYNAME_PATHOGENGROUP withKey:PG_PRESENTATIONID withValue:theUUID withMOC:nil];
    return entities;
}


//Returns the uuid of the object
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer
{
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *uuid = [theAttributeDictionary valueForKey:PG_UUID];
    NSDate *modifedDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:PG_MODIFIEDDATE]];
    
    BDPathogenGroup *pathogenGroup = [BDPathogenGroup retrieveWithUUID:uuid];
    if(nil == pathogenGroup)
    {
        NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
        NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_PATHOGENGROUP
                                                  inManagedObjectContext:moc];
        
        pathogenGroup = [[[BDPathogenGroup alloc] initWithEntity:entity insertIntoManagedObjectContext:moc] autorelease];
        pathogenGroup.uuid = uuid;
        
    }
    
    NSLog(@"Local Pathogen Group Modified Date:%@", [dateFormatter stringFromDate:pathogenGroup.modifiedDate]);
    NSLog(@"Repository Pathogen Modified Date:%@", [dateFormatter stringFromDate:modifedDate]);
    
    if ( (nil == pathogenGroup.modifiedDate) 
        || ( ([pathogenGroup.modifiedDate compare:modifedDate] == NSOrderedAscending) || (overwriteNewer) ) )
    {
        int schemaVersion = [[theAttributeDictionary valueForKey:PG_SCHEMAVERSION] intValue];
        
        pathogenGroup.schemaVersion = [NSNumber numberWithInt:schemaVersion];
        
        switch (schemaVersion)
        {
            case 1:
            default:
            {
                pathogenGroup.createdBy = [theAttributeDictionary valueForKey:PG_CREATEDBY];
                pathogenGroup.createdDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:PG_CREATEDDATE]];
                pathogenGroup.modifiedBy  = [theAttributeDictionary valueForKey:PG_MODIFIEDBY];
                pathogenGroup.modifiedDate = modifedDate;
                pathogenGroup.inUseBy = [theAttributeDictionary valueForKey:PG_INUSEBY];
                pathogenGroup.deprecated = [NSNumber numberWithBool:[[theAttributeDictionary valueForKey:PG_DEPRECATED] boolValue]];
                pathogenGroup.displayOrder = [NSNumber numberWithInt:[[theAttributeDictionary valueForKey:PG_DISPLAYORDER] intValue]];
                
                pathogenGroup.presentationId = [theAttributeDictionary valueForKey:PG_PRESENTATIONID];
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
    self.inUseBy = @""; //Review when this gets toggled. Perhaps on push
    self.modifiedDate = [NSDate date];
    self.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    
    [BDQueueEntry createWithObjectUuid:self.uuid 
                        withEntityName:ENTITYNAME_PATHOGENGROUP 
                            withAction:UPDATE_QueueEntryActionType 
                              withSave:NO];
    
    [[DataController sharedInstance] saveContext];
}


@end
