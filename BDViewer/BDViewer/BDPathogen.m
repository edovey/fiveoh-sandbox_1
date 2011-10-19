//
//  Pathogen.m
//  BDViewer
//
//  Created by Liz Dovey on 11-09-26.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "BDPathogen.h"
#import "NSString+UUID.h"
#import "BDQueueEntry.h"


@implementation BDPathogen
@dynamic createdBy;
@dynamic createdDate;
@dynamic deprecated;
@dynamic inUseBy;
@dynamic modifiedBy;
@dynamic modifiedDate;
@dynamic displayOrder;
@dynamic name;
@dynamic pathogenGroupId;
@dynamic schemaVersion;
@dynamic uuid;

+(NSString *)create
{
    NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
    NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_PATHOGEN inManagedObjectContext:moc];
    
	BDPathogen *pathogen = [[BDPathogen alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
    
    pathogen.uuid = [NSString UUIDCreate];
    pathogen.createdDate = [NSDate date];
    pathogen.createdBy = [[UIDevice currentDevice] uniqueIdentifier];
    pathogen.modifiedDate = pathogen.createdDate;
    pathogen.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    pathogen.inUseBy = nil;
    pathogen.schemaVersion = [NSNumber numberWithInt:[SCHEMAVERSION_PATHOGEN intValue]];
    pathogen.deprecated = [NSNumber numberWithBool:NO];
    pathogen.displayOrder = [NSNumber numberWithInt:-1];
      
    [BDQueueEntry createWithObjectUuid:pathogen.uuid 
                      withEntityName:ENTITYNAME_PATHOGEN 
                          withAction:CREATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
    NSString *uuid = pathogen.uuid;
    [pathogen release];
    
    return uuid;
}

+(BDPathogen *)retrieveWithUUID:(NSString *)theUUID
{
    return (BDPathogen *)[[DataController sharedInstance] retrieveManagedObject:ENTITYNAME_PATHOGEN 
                                                                           uuid:theUUID 
                                                                      targetMOC:nil];
}

+(NSArray *) retrieveAllWithParentUUID:(NSString *)theUUID parentPropertyName:(NSString *)thePropertyName
{
    NSArray *entities = [[DataController sharedInstance] retrieveManagedObjectsForValue:ENTITYNAME_PATHOGEN withKey:PA_PATHOGENGROUPID withValue:theUUID withMOC:nil];
    return entities;
}


//Returns the uuid of the object
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer
{
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *uuid = [theAttributeDictionary valueForKey:PA_UUID];
    NSDate *modifedDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:PA_MODIFIEDDATE]];
    
    BDPathogen *pathogen = [BDPathogen retrieveWithUUID:uuid];
    if(nil == pathogen)
    {
        NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
        NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_PATHOGEN 
                                                  inManagedObjectContext:moc];
        
        pathogen = [[[BDPathogen alloc] initWithEntity:entity insertIntoManagedObjectContext:moc]autorelease];
        pathogen.uuid = uuid;
        
    }
    
    NSLog(@"Local Pathogen Modified Date:%@", [dateFormatter stringFromDate:pathogen.modifiedDate]);
    NSLog(@"Repository Pathogen Modified Date:%@", [dateFormatter stringFromDate:modifedDate]);
    
    if ( (nil == pathogen.modifiedDate) 
        || ( ([pathogen.modifiedDate compare:modifedDate] == NSOrderedAscending) || (overwriteNewer) ) )
    {
        int schemaVersion = [[theAttributeDictionary valueForKey:PA_SCHEMAVERSION] intValue];
        
        pathogen.schemaVersion = [NSNumber numberWithInt:schemaVersion];
        
        switch (schemaVersion)
        {
            case 1:
            default:
            {
                pathogen.createdBy = [theAttributeDictionary valueForKey:PA_CREATEDBY];
                pathogen.createdDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:PA_CREATEDDATE]];
                pathogen.modifiedBy  = [theAttributeDictionary valueForKey:PA_MODIFIEDBY];
                pathogen.modifiedDate = modifedDate;
                pathogen.inUseBy = [theAttributeDictionary valueForKey:PA_INUSEBY];
                pathogen.deprecated = [NSNumber numberWithBool:[[theAttributeDictionary valueForKey:PA_DEPRECATED] boolValue]];
                pathogen.displayOrder = [NSNumber numberWithInt:[[theAttributeDictionary valueForKey:PA_DISPLAYORDER] intValue]];
                
                pathogen.pathogenGroupId = [theAttributeDictionary valueForKey:PA_PATHOGENGROUPID];
                pathogen.name = [theAttributeDictionary valueForKey:PA_NAME];
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
                      withEntityName:ENTITYNAME_PATHOGEN 
                          withAction:UPDATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
}


@end
