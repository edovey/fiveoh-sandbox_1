//
//  Section.m
//  fiveoh-one
//
//  Created by GrubbyHedgehog on 11-09-19.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "Section.h"
#import "NSString+UUID.h"
#import "QueueEntry.h"

@implementation Section

@dynamic uuid;
@dynamic name;
@dynamic createdBy;
@dynamic createdDate;
@dynamic deprecated;
@dynamic inUseBy;
@dynamic modifiedBy;
@dynamic modifiedDate;
@dynamic schemaVersion;


+(NSString *)create
{
    NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
    NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_SECTION inManagedObjectContext:moc];
    
	Section  *section = [[Section alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
    
    section.uuid = [NSString UUIDCreate];
    section.createdDate = [NSDate date];
    section.createdBy = [[UIDevice currentDevice] uniqueIdentifier];
    section.modifiedDate = section.createdDate;
    section.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    section.inUseBy = nil;
    section.schemaVersion = [NSNumber numberWithInt:[SCHEMAVERSION_SECTION intValue]];
    section.deprecated = [NSNumber numberWithBool:NO];
        
    [QueueEntry createWithObjectUuid:section.uuid 
                      withEntityName:ENTITYNAME_SECTION
                          withAction:CREATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
    NSString *uuid = section.uuid;
    
    return uuid;
}

+(Section *)retrieveWithUUID:(NSString *)theUUID
{
    return (Section *)[[DataController sharedInstance] retrieveManagedObject:ENTITYNAME_SECTION
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
    
    NSString *uuid = [theAttributeDictionary valueForKey:SN_UUID];
    NSDate *modifedDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:SN_MODIFIEDDATE]];
    
    Section *section = [Section retrieveWithUUID:uuid];
    if(nil == section)
    {
        NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
        NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_SECTION
                                                  inManagedObjectContext:moc];
        
        section = [[Section alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
        section.uuid = uuid;
        
    }
    
    NSLog(@"Local Section Modified Date:%@", [dateFormatter stringFromDate:section.modifiedDate]);
    NSLog(@"Repository Section Modified Date:%@", [dateFormatter stringFromDate:modifedDate]);
    
    if ( (nil == section.modifiedDate) 
        || ( ([section.modifiedDate compare:modifedDate] == NSOrderedAscending) || (overwriteNewer) ) )
    {
        int schemaVersion = [[theAttributeDictionary valueForKey:SN_SCHEMAVERSION] intValue];
        
        section.schemaVersion = [NSNumber numberWithInt:schemaVersion];
        
        switch (schemaVersion)
        {
            case 1:
            default:
            {
                section.createdBy = [theAttributeDictionary valueForKey:SN_CREATEDBY];
                section.createdDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:SN_CREATEDDATE]];
                section.modifiedBy  = [theAttributeDictionary valueForKey:SN_MODIFIEDBY];
                section.modifiedDate = modifedDate;
                section.inUseBy = [theAttributeDictionary valueForKey:SN_INUSEBY];
                section.deprecated = [NSNumber numberWithBool:[[theAttributeDictionary valueForKey:SN_DEPRECATED] boolValue]];
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
    
    
    
    [QueueEntry createWithObjectUuid:self.uuid 
                      withEntityName:ENTITYNAME_SECTION 
                          withAction:UPDATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
}


@end
