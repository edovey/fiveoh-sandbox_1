//
//  BDChapter.m
//  BDViewer
//
//  Created by Liz Dovey on 11-10-18.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "BDChapter.h"
#import "NSString+UUID.h"
#import "BDQueueEntry.h"
#import "BDSection.h"
@implementation BDChapter

@dynamic createdBy;
@dynamic createdDate;
@dynamic deprecated;
@dynamic displayOrder;
@dynamic inUseBy;
@dynamic modifiedBy;
@dynamic modifiedDate;
@dynamic name;
@dynamic schemaVersion;
@dynamic uuid;

+(NSString *)create
{
    NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
    NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_CHAPTER inManagedObjectContext:moc];
    
	BDChapter  *chapter = [[BDChapter alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
    
    chapter.uuid = [NSString UUIDCreate];
    chapter.createdDate = [NSDate date];
    chapter.createdBy = [[UIDevice currentDevice] uniqueIdentifier];
    chapter.modifiedDate = chapter.createdDate;
    chapter.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    chapter.inUseBy = nil;
    chapter.schemaVersion = [NSNumber numberWithInt:[SCHEMAVERSION_CHAPTER intValue]];
    chapter.deprecated = [NSNumber numberWithBool:NO];
    chapter.displayOrder = [NSNumber numberWithInt:-1];
    
    [BDQueueEntry createWithObjectUuid:chapter.uuid 
                        withEntityName:ENTITYNAME_CHAPTER
                            withAction:CREATE_QueueEntryActionType 
                              withSave:NO];
    
    [[DataController sharedInstance] saveContext];
    NSString *uuid = chapter.uuid;
    [chapter release];
    
    return uuid;
}

+(BDChapter *)retrieveWithUUID:(NSString *)theUUID
{
    return (BDChapter *)[[DataController sharedInstance] retrieveManagedObject:ENTITYNAME_CHAPTER
                                                                          uuid:theUUID 
                                                                     targetMOC:nil];
}

+(NSArray *) retrieveAll 
{
    NSMutableArray * allChapters = [[DataController sharedInstance] allInstancesOf:ENTITYNAME_CHAPTER orderedBy:CH_DISPLAYORDER loadData:false targetMOC:nil];
    return [NSArray arrayWithArray:allChapters];
}

//Returns the uuid of the object
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer
{
    NSDateFormatter* dateFormatter = [[NSDateFormatter alloc] init];
	[dateFormatter setTimeStyle:NSDateFormatterFullStyle];
	[dateFormatter setDateFormat:DATETIMEFORMAT];
    
    NSString *uuid = [theAttributeDictionary valueForKey:CH_UUID];
    NSDate *modifedDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:CH_MODIFIEDDATE]];
    
    BDChapter *chapter = [BDChapter retrieveWithUUID:uuid];
    if(nil == chapter)
    {
        NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
        NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_CHAPTER
                                                  inManagedObjectContext:moc];
        
        chapter = [[[BDChapter alloc] initWithEntity:entity insertIntoManagedObjectContext:moc] autorelease];
        chapter.uuid = uuid;
        
    }
    
    NSLog(@"Local Section Modified Date:%@", [dateFormatter stringFromDate:chapter.modifiedDate]);
    NSLog(@"Repository Section Modified Date:%@", [dateFormatter stringFromDate:modifedDate]);
    
    if ( (nil == chapter.modifiedDate) 
        || ( ([chapter.modifiedDate compare:modifedDate] == NSOrderedAscending) || (overwriteNewer) ) )
    {
        int schemaVersion = [[theAttributeDictionary valueForKey:CH_SCHEMAVERSION] intValue];
        
        chapter.schemaVersion = [NSNumber numberWithInt:schemaVersion];
        
        switch (schemaVersion)
        {
            case 1:
            default:
            {
                chapter.createdBy = [theAttributeDictionary valueForKey:CH_CREATEDBY];
                chapter.createdDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:CH_CREATEDDATE]];
                chapter.modifiedBy  = [theAttributeDictionary valueForKey:CH_MODIFIEDBY];
                chapter.modifiedDate = modifedDate;
                chapter.inUseBy = [theAttributeDictionary valueForKey:CH_INUSEBY];
                chapter.deprecated = [NSNumber numberWithBool:[[theAttributeDictionary valueForKey:CH_DEPRECATED] boolValue]];
                chapter.displayOrder = [NSNumber numberWithInt:[[theAttributeDictionary valueForKey:CH_DISPLAYORDER] intValue]];
                
                chapter.name = [theAttributeDictionary valueForKey:CH_NAME];
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
                        withEntityName:ENTITYNAME_CHAPTER 
                            withAction:UPDATE_QueueEntryActionType 
                              withSave:NO];
    
    [[DataController sharedInstance] saveContext];
}

@end
