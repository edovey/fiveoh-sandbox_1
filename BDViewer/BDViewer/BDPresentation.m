//
//  Presentation.m
//  BDViewer
//
//  Created by Liz Dovey on 11-09-26.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "BDPresentation.h"
#import "NSString+UUID.h"
#import "BDQueueEntry.h"


@implementation BDPresentation
@dynamic createdBy;
@dynamic createdDate;
@dynamic deprecated;
@dynamic diseaseId;
@dynamic inUseBy;
@dynamic modifiedBy;
@dynamic modifiedDate;
@dynamic name;
@dynamic displayOrder;
@dynamic schemaVersion;
@dynamic uuid;

+(NSString *)create
{
    NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
    NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_PRESENTATION inManagedObjectContext:moc];
    
	BDPresentation *presentation = [[BDPresentation alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
    
    presentation.uuid = [NSString UUIDCreate];
    presentation.createdDate = [NSDate date];
    presentation.createdBy = [[UIDevice currentDevice] uniqueIdentifier];
    presentation.modifiedDate = presentation.createdDate;
    presentation.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    presentation.inUseBy = nil;
    presentation.schemaVersion = [NSNumber numberWithInt:[SCHEMAVERSION_PRESENTATION intValue]];
    presentation.deprecated = [NSNumber numberWithBool:NO];
    presentation.displayOrder = [NSNumber numberWithInt:-1];
    
    [BDQueueEntry createWithObjectUuid:presentation.uuid 
                      withEntityName:ENTITYNAME_PRESENTATION 
                          withAction:CREATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
    NSString *uuid = presentation.uuid;
    [presentation release];
    
    return uuid;
}

+(BDPresentation *)retrieveWithUUID:(NSString *)theUUID
{
    return (BDPresentation *)[[DataController sharedInstance] retrieveManagedObject:ENTITYNAME_PRESENTATION
                                                                           uuid:theUUID 
                                                                      targetMOC:nil];
}

+(NSArray *) retrieveAllWithParentUUID:(NSString *)theUUID
{
    NSArray *presentationsInDisease = [[DataController sharedInstance] retrieveManagedObjectsForValue:ENTITYNAME_PRESENTATION withKey:@"diseaseId" withValue:theUUID orderedBy: @"displayOrder" withMOC:nil];
    return presentationsInDisease;
}


+(NSNumber *) retrieveCountWithParentUUID:(NSString *)theUUID;
{
    NSMutableArray *predicateArray = [[NSMutableArray alloc] initWithCapacity:0];
    NSPredicate *parentPredicate = [NSPredicate predicateWithFormat:@"diseaseId = %@", theUUID];
    [predicateArray addObject:parentPredicate];
    NSPredicate *predicate = [NSCompoundPredicate andPredicateWithSubpredicates:predicateArray];
    NSNumber *count = [[DataController sharedInstance] aggregateOperation:@"count:" onEntity: ENTITYNAME_PRESENTATION onAttribute:@"diseaseId" withPredicate:predicate];
    return count;
}

//Returns the uuid of the object
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer
{
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *uuid = [theAttributeDictionary valueForKey:PR_UUID];
    NSDate *modifedDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:PR_MODIFIEDDATE]];
    
    BDPresentation *presentation = [BDPresentation retrieveWithUUID:uuid];
    if(nil == presentation)
    {
        NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
        NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_PRESENTATION 
                                                  inManagedObjectContext:moc];
        
        presentation = [[[BDPresentation alloc] initWithEntity:entity insertIntoManagedObjectContext:moc] autorelease];
        presentation.uuid = uuid;
        
    }
    
    NSLog(@"Local Presentation Modified Date:%@", [dateFormatter stringFromDate:presentation.modifiedDate]);
    NSLog(@"Repository Presentation Modified Date:%@", [dateFormatter stringFromDate:modifedDate]);
    
    if ( (nil == presentation.modifiedDate) 
        || ( ([presentation.modifiedDate compare:modifedDate] == NSOrderedAscending) || (overwriteNewer) ) )
    {
        int schemaVersion = [[theAttributeDictionary valueForKey:PR_SCHEMAVERSION] intValue];
        
        presentation.schemaVersion = [NSNumber numberWithInt:schemaVersion];
        
        switch (schemaVersion)
        {
            case 1:
            default:
            {
                presentation.createdBy = [theAttributeDictionary valueForKey:PR_CREATEDBY];
                presentation.createdDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:PR_CREATEDDATE]];
                presentation.modifiedBy  = [theAttributeDictionary valueForKey:PR_MODIFIEDBY];
                presentation.modifiedDate = modifedDate;
                presentation.inUseBy = [theAttributeDictionary valueForKey:PR_INUSEBY];
                presentation.deprecated = [NSNumber numberWithBool:[[theAttributeDictionary valueForKey:PR_DEPRECATED] boolValue]];
                
                presentation.diseaseId = [theAttributeDictionary valueForKey:PR_DISEASEID];
                presentation.displayOrder = [NSNumber numberWithInt:[[theAttributeDictionary valueForKey:PR_DISPLAYORDER] intValue]];
                presentation.name = [theAttributeDictionary valueForKey:PR_NAME];
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
    [dateFormatter release];
    
    return uuid;
}

-(void)commitChanges
{
    self.inUseBy = @""; //Review when this gets toggled. Perhaps on push
    self.modifiedDate = [NSDate date];
    self.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    
    [BDQueueEntry createWithObjectUuid:self.uuid 
                      withEntityName:ENTITYNAME_PRESENTATION 
                          withAction:UPDATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
}


@end

