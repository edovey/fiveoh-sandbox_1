//
//  Category.m
//  fiveoh-one
//
//  Created by GrubbyHedgehog on 11-09-19.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "Category.h"
#import "NSString+UUID.h"
#import "QueueEntry.h"

@implementation Category

@dynamic uuid;
@dynamic sectionId;
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
    NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_CATEGORY inManagedObjectContext:moc];
    
	Category *category = [[Category alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
    
    category.uuid = [NSString UUIDCreate];
    category.createdDate = [NSDate date];
    category.createdBy = [[UIDevice currentDevice] uniqueIdentifier];
    category.modifiedDate = category.createdDate;
    category.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    category.inUseBy = nil;
    category.schemaVersion = [NSNumber numberWithInt:[SCHEMAVERSION_CATEGORY intValue]];
    category.deprecated = [NSNumber numberWithBool:NO];
        
    [QueueEntry createWithObjectUuid:category.uuid 
                      withEntityName:ENTITYNAME_CATEGORY 
                          withAction:CREATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
    NSString *uuid = category.uuid;
    
    return uuid;
}

+(Category *)retrieveWithUUID:(NSString *)theUUID
{
    return (Category *)[[DataController sharedInstance] retrieveManagedObject:ENTITYNAME_CATEGORY 
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
    
    NSString *uuid = [theAttributeDictionary valueForKey:CT_UUID];
    NSDate *modifedDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:CT_MODIFIEDDATE]];
    
    Category *category  = [Category retrieveWithUUID:uuid];
    if(nil == category)
    {
        NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
        NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_CATEGORY 
                                                  inManagedObjectContext:moc];
        
        category = [[Category alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
        category.uuid = uuid;
        
    }
    
    NSLog(@"Local Category Modified Date:%@", [dateFormatter stringFromDate:category.modifiedDate]);
    NSLog(@"Repository Category Modified Date:%@", [dateFormatter stringFromDate:modifedDate]);
    
    if ( (nil == category.modifiedDate) 
        || ( ([category.modifiedDate compare:modifedDate] == NSOrderedAscending) || (overwriteNewer) ) )
    {
        int schemaVersion = [[theAttributeDictionary valueForKey:CT_SCHEMAVERSION] intValue];
        
        category.schemaVersion = [NSNumber numberWithInt:schemaVersion];
        
        switch (schemaVersion)
        {
            case 1:
            default:
            {
                category.name = [theAttributeDictionary valueForKey:CT_NAME];
                category.sectionId = [theAttributeDictionary valueForKey:CT_SECTIONID];
                category.createdBy = [theAttributeDictionary valueForKey:CT_CREATEDBY];
                category.createdDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:CT_CREATEDDATE]];
                category.modifiedBy  = [theAttributeDictionary valueForKey:CT_MODIFIEDBY];
                category.modifiedDate = modifedDate;
                category.inUseBy = [theAttributeDictionary valueForKey:CT_INUSEBY];
                category.deprecated = [NSNumber numberWithBool:[[theAttributeDictionary valueForKey:CT_DEPRECATED] boolValue]];
                category.sectionId = [theAttributeDictionary valueForKey:CT_SECTIONID];
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
                      withEntityName:ENTITYNAME_CATEGORY 
                          withAction:UPDATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
}


@end

