//
//  TherapyGroup.m
//  BDViewer
//
//  Created by Liz Dovey on 11-09-26.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "BDTherapyGroup.h"
#import "NSString+UUID.h"
#import "BDQueueEntry.h"


@implementation BDTherapyGroup
@dynamic createdBy;
@dynamic createdDate;
@dynamic deprecated;
@dynamic displayOrder;
@dynamic inUseBy;
@dynamic modifiedBy;
@dynamic modifiedDate;
@dynamic name;
@dynamic pathogenGroupId;
@dynamic schemaVersion;
@dynamic uuid;

+(NSString *)create
{
    NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
    NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_THERAPYGROUP inManagedObjectContext:moc];
    
	BDTherapyGroup *therapyGroup = [[BDTherapyGroup alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
    
    therapyGroup.uuid = [NSString UUIDCreate];
    therapyGroup.createdDate = [NSDate date];
    therapyGroup.createdBy = [[UIDevice currentDevice] uniqueIdentifier];
    therapyGroup.modifiedDate = therapyGroup.createdDate;
    therapyGroup.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    therapyGroup.inUseBy = nil;
    therapyGroup.schemaVersion = [NSNumber numberWithInt:[SCHEMAVERSION_THERAPYGROUP intValue]];
    therapyGroup.deprecated = [NSNumber numberWithBool:NO];
    therapyGroup.displayOrder = [NSNumber numberWithInt:-1];
    
    [BDQueueEntry createWithObjectUuid:therapyGroup.uuid 
                      withEntityName:ENTITYNAME_THERAPYGROUP 
                          withAction:CREATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
    NSString *uuid = therapyGroup.uuid;
    [therapyGroup release];
    
    return uuid;
}

+(BDTherapyGroup *)retrieveWithUUID:(NSString *)theUUID
{
    return (BDTherapyGroup *)[[DataController sharedInstance] retrieveManagedObject:ENTITYNAME_THERAPYGROUP
                                                                           uuid:theUUID 
                                                                      targetMOC:nil];
}

+(NSArray *) retrieveAllWithParentUUID:(NSString *)theUUID
{
    NSArray *entities = [[DataController sharedInstance] retrieveManagedObjectsForValue:ENTITYNAME_THERAPYGROUP withKey:@"pathogenGroupId" withValue:theUUID orderedBy:@"displayOrder" withMOC:nil];
    return entities;
}

+(NSNumber *) retrieveCountWithParentUUID:(NSString *)theUUID
{
    NSMutableArray *predicateArray = [[NSMutableArray alloc] initWithCapacity:0];
    NSPredicate *parentPredicate = [NSPredicate predicateWithFormat:@"chapterId = %@", theUUID];
    [predicateArray addObject:parentPredicate];
    NSPredicate *predicate = [NSCompoundPredicate andPredicateWithSubpredicates:predicateArray];
    [predicateArray release];
    NSNumber *count = [[DataController sharedInstance] aggregateOperation:@"count:" onEntity: ENTITYNAME_THERAPYGROUP onAttribute:@"chapterId" withPredicate:predicate];
    return count;
}



//Returns the uuid of the object
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer
{
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *uuid = [theAttributeDictionary valueForKey:TG_UUID];
    NSDate *modifedDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:TG_MODIFIEDDATE]];
    
    BDTherapyGroup *therapyGroup = [BDTherapyGroup retrieveWithUUID:uuid];
    if(nil == therapyGroup)
    {
        NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
        NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_THERAPYGROUP 
                                                  inManagedObjectContext:moc];
        
        therapyGroup = [[[BDTherapyGroup alloc] initWithEntity:entity insertIntoManagedObjectContext:moc] autorelease];
        therapyGroup.uuid = uuid;
        
    }
    
    NSLog(@"Local Therapy Group Modified Date:%@", [dateFormatter stringFromDate:therapyGroup.modifiedDate]);
    NSLog(@"Repository Therapy Group Modified Date:%@", [dateFormatter stringFromDate:modifedDate]);
    
    if ( (nil == therapyGroup.modifiedDate) 
        || ( ([therapyGroup.modifiedDate compare:modifedDate] == NSOrderedAscending) || (overwriteNewer) ) )
    {
        int schemaVersion = [[theAttributeDictionary valueForKey:TG_SCHEMAVERSION] intValue];
        
        therapyGroup.schemaVersion = [NSNumber numberWithInt:schemaVersion];
        
        switch (schemaVersion)
        {
            case 1:
            default:
            {
                therapyGroup.createdBy = [theAttributeDictionary valueForKey:TG_CREATEDBY];
                therapyGroup.createdDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:TG_CREATEDDATE]];
                therapyGroup.modifiedBy  = [theAttributeDictionary valueForKey:TG_MODIFIEDBY];
                therapyGroup.modifiedDate = modifedDate;
                therapyGroup.inUseBy = [theAttributeDictionary valueForKey:TG_INUSEBY];
                therapyGroup.deprecated = [NSNumber numberWithBool:[[theAttributeDictionary valueForKey:TG_DEPRECATED] boolValue]];
                
                therapyGroup.pathogenGroupId = [theAttributeDictionary valueForKey:TG_PATHOGENGROUPID];
                therapyGroup.displayOrder = [NSNumber numberWithInt:[[theAttributeDictionary valueForKey:TG_DISPLAYORDER] intValue]];
                therapyGroup.name = [theAttributeDictionary valueForKey:TG_NAME];
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
    
    [BDQueueEntry createWithObjectUuid:self.uuid 
                      withEntityName:ENTITYNAME_THERAPYGROUP 
                          withAction:UPDATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
}


@end
