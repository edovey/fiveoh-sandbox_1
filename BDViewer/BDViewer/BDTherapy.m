//
//  Therapy.m
//  BDViewer
//
//  Created by Liz Dovey on 11-09-26.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "BDTherapy.h"
#import "NSString+UUID.h"
#import "BDQueueEntry.h"


@implementation BDTherapy
@dynamic createdBy;
@dynamic createdDate;
@dynamic deprecated;
@dynamic dosage;
@dynamic duration;
@dynamic inUseBy;
@dynamic modifiedBy;
@dynamic modifiedDate;
@dynamic name;
@dynamic schemaVersion;
@dynamic therapyGroupId;
@dynamic therapyGroupJoinType;
@dynamic displayOrder;
@dynamic uuid;

+(NSString *)create
{
    NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
    NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_THERAPY inManagedObjectContext:moc];
    
	BDTherapy *therapy = [[BDTherapy alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
    
    therapy.uuid = [NSString UUIDCreate];
    therapy.createdDate = [NSDate date];
    therapy.createdBy = [[UIDevice currentDevice] uniqueIdentifier];
    therapy.modifiedDate = therapy.createdDate;
    therapy.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    therapy.inUseBy = nil;
    therapy.schemaVersion = [NSNumber numberWithInt:[SCHEMAVERSION_THERAPY intValue]];
    therapy.deprecated = [NSNumber numberWithBool:NO];
    
    [BDQueueEntry createWithObjectUuid:therapy.uuid 
                      withEntityName:ENTITYNAME_THERAPY 
                          withAction:CREATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
    NSString *uuid = therapy.uuid;
    
    return uuid;
}

+(BDTherapy *)retrieveWithUUID:(NSString *)theUUID
{
    return (BDTherapy *)[[DataController sharedInstance] retrieveManagedObject:ENTITYNAME_THERAPY 
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
    
    NSString *uuid = [theAttributeDictionary valueForKey:TH_UUID];
    NSDate *modifedDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:TH_MODIFIEDDATE]];
    
    BDTherapy *therapy = [BDTherapy retrieveWithUUID:uuid];
    if(nil == therapy)
    {
        NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
        NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_THERAPY 
                                                  inManagedObjectContext:moc];
        
        therapy = [[BDTherapy alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
        therapy.uuid = uuid;
        
    }
    
    NSLog(@"Local Therapy Modified Date:%@", [dateFormatter stringFromDate:therapy.modifiedDate]);
    NSLog(@"Repository Therapy Modified Date:%@", [dateFormatter stringFromDate:modifedDate]);
    
    if ( (nil == therapy.modifiedDate) 
        || ( ([therapy.modifiedDate compare:modifedDate] == NSOrderedAscending) || (overwriteNewer) ) )
    {
        int schemaVersion = [[theAttributeDictionary valueForKey:TH_SCHEMAVERSION] intValue];
        
        therapy.schemaVersion = [NSNumber numberWithInt:schemaVersion];
        
        switch (schemaVersion)
        {
            case 1:
            default:
            {
                therapy.createdBy = [theAttributeDictionary valueForKey:TH_CREATEDBY];
                therapy.createdDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:TH_CREATEDDATE]];
                therapy.modifiedBy  = [theAttributeDictionary valueForKey:TH_MODIFIEDBY];
                therapy.modifiedDate = modifedDate;
                therapy.inUseBy = [theAttributeDictionary valueForKey:TH_INUSEBY];
                therapy.deprecated = [NSNumber numberWithBool:[[theAttributeDictionary valueForKey:TH_DEPRECATED] boolValue]];
                
                therapy.therapyGroupId = [theAttributeDictionary valueForKey:TH_THERAPYGROUPID];
                therapy.therapyGroupJoinType = [theAttributeDictionary valueForKey:TH_THERAPYGROUPJOINTYPE];
                therapy.name = [theAttributeDictionary valueForKey:TH_NAME];
                therapy.dosage = [theAttributeDictionary valueForKey:TH_DOSAGE];
                therapy.duration = [theAttributeDictionary valueForKey:TH_DURATION];
                therapy.displayOrder = [theAttributeDictionary valueForKey:TH_DISPLAYORDER];
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

-(void)commitChanges
{
    self.inUseBy = @""; //Review when this gets toggled. Perhaps on push
    self.modifiedDate = [NSDate date];
    self.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    
   
    [BDQueueEntry createWithObjectUuid:self.uuid 
                      withEntityName:ENTITYNAME_THERAPY
                          withAction:UPDATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
}


@end
