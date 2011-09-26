//
//  Presentation.m
//  BDViewer
//
//  Created by Liz Dovey on 11-09-26.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "Presentation.h"
#import "NSString+UUID.h"
#import "QueueEntry.h"


@implementation Presentation
@dynamic createdBy;
@dynamic createdDate;
@dynamic deprecated;
@dynamic diseaseId;
@dynamic inUseBy;
@dynamic modifiedBy;
@dynamic modifiedDate;
@dynamic name;
@dynamic overview;
@dynamic displayOrder;
@dynamic schemaVersion;
@dynamic uuid;

+(NSString *)create
{
    NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
    NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_PRESENTATION inManagedObjectContext:moc];
    
	Presentation *presentation = [[Presentation alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
    
    presentation.uuid = [NSString UUIDCreate];
    presentation.createdDate = [NSDate date];
    presentation.createdBy = [[UIDevice currentDevice] uniqueIdentifier];
    presentation.modifiedDate = presentation.createdDate;
    presentation.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    presentation.inUseBy = nil;
    presentation.schemaVersion = [NSNumber numberWithInt:[SCHEMAVERSION_PRESENTATION intValue]];
    presentation.deprecated = [NSNumber numberWithBool:NO];
    
    [QueueEntry createWithObjectUuid:presentation.uuid 
                      withEntityName:ENTITYNAME_PRESENTATION 
                          withAction:CREATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
    NSString *uuid = presentation.uuid;
    
    return uuid;
}

+(Presentation *)retrieveWithUUID:(NSString *)theUUID
{
    return (Presentation *)[[DataController sharedInstance] retrieveManagedObject:ENTITYNAME_PRESENTATION
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
    
    NSString *uuid = [theAttributeDictionary valueForKey:PR_UUID];
    NSDate *modifedDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:PR_MODIFIEDDATE]];
    
    Presentation *presentation = [Presentation retrieveWithUUID:uuid];
    if(nil == presentation)
    {
        NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
        NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_PRESENTATION 
                                                  inManagedObjectContext:moc];
        
        presentation = [[Presentation alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
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
                presentation.overview = [theAttributeDictionary valueForKey:PR_OVERVIEW];
                presentation.displayOrder = [theAttributeDictionary valueForKey:PR_DISPLAYORDER];
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
    
    return uuid;
}

-(void)commitChanges
{
    self.inUseBy = @""; //Review when this gets toggled. Perhaps on push
    self.modifiedDate = [NSDate date];
    self.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    
    [QueueEntry createWithObjectUuid:self.uuid 
                      withEntityName:ENTITYNAME_PRESENTATION 
                          withAction:UPDATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
}


@end

