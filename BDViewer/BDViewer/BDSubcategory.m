//
//  Subcategory.m
//  BDViewer
//
//  Created by GrubbyHedgehog on 11-09-19.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "BDSubcategory.h"
#import "NSString+UUID.h"
#import "BDQueueEntry.h"


@implementation BDSubcategory

@dynamic uuid;
@dynamic name;
@dynamic categoryId;
@dynamic createdBy;
@dynamic createdDate;
@dynamic deprecated;
@dynamic inUseBy;
@dynamic modifiedBy;
@dynamic modifiedDate;
@dynamic schemaVersion;
@dynamic displayOrder;

+(NSString *)create
{
    NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
    NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_SUBCATEGORY inManagedObjectContext:moc];
    
	BDSubcategory *subcategory = [[BDSubcategory alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
    
    subcategory.uuid = [NSString UUIDCreate];
    subcategory.name = nil;
    subcategory.categoryId = nil;
    subcategory.createdDate = [NSDate date];
    subcategory.createdBy = [[UIDevice currentDevice] uniqueIdentifier];
    subcategory.modifiedDate = subcategory.createdDate;
    subcategory.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    subcategory.inUseBy = nil;
    subcategory.schemaVersion = [NSNumber numberWithInt:[SCHEMAVERSION_SUBCATEGORY intValue]];
    subcategory.deprecated = [NSNumber numberWithBool:NO];
    subcategory.displayOrder = [NSNumber numberWithInt:-1];
    

    [BDQueueEntry createWithObjectUuid:subcategory.uuid 
                      withEntityName:ENTITYNAME_SUBCATEGORY 
                          withAction:CREATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
    NSString *uuid = subcategory.uuid;
    [subcategory release];
    
    return uuid;
}

+(BDSubcategory *)retrieveWithUUID:(NSString *)theUUID
{
    return (BDSubcategory *)[[DataController sharedInstance] retrieveManagedObject:ENTITYNAME_SUBCATEGORY 
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
    
    NSString *uuid = [theAttributeDictionary valueForKey:SC_UUID];
    NSDate *modifedDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:SC_MODIFIEDDATE]];
    
    BDSubcategory *subcategory = [BDSubcategory retrieveWithUUID:uuid];
    if(nil == subcategory)
    {
        NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
        NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_SUBCATEGORY 
                                                  inManagedObjectContext:moc];
        
        subcategory = [[[BDSubcategory alloc] initWithEntity:entity insertIntoManagedObjectContext:moc] autorelease];
        subcategory.uuid = uuid;
        
    }
    
    NSLog(@"Local Subcategory Modified Date:%@", [dateFormatter stringFromDate:subcategory.modifiedDate]);
    NSLog(@"Repository Subcategory Modified Date:%@", [dateFormatter stringFromDate:modifedDate]);
    
    if ( (nil == subcategory.modifiedDate) 
        || ( ([subcategory.modifiedDate compare:modifedDate] == NSOrderedAscending) || (overwriteNewer) ) )
    {
        int schemaVersion = [[theAttributeDictionary valueForKey:SC_SCHEMAVERSION] intValue];
        
        subcategory.schemaVersion = [NSNumber numberWithInt:schemaVersion];
        
        switch (schemaVersion)
        {
            case 1:
            default:
            {
                subcategory.createdBy = [theAttributeDictionary valueForKey:SC_CREATEDBY];
                subcategory.createdDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:SC_CREATEDDATE]];
                subcategory.modifiedBy  = [theAttributeDictionary valueForKey:SC_MODIFIEDBY];
                subcategory.modifiedDate = modifedDate;
                subcategory.inUseBy = [theAttributeDictionary valueForKey:SC_INUSEBY];
                subcategory.deprecated = [NSNumber numberWithBool:[[theAttributeDictionary valueForKey:SC_DEPRECATED] boolValue]];
                subcategory.displayOrder = [NSNumber numberWithInt:[[theAttributeDictionary valueForKey:SC_DISPLAYORDER] boolValue]];

                subcategory.categoryId = [theAttributeDictionary valueForKey:SC_CATEGORYID];
                subcategory.name = [theAttributeDictionary valueForKey:SC_NAME];
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
                      withEntityName:ENTITYNAME_SUBCATEGORY 
                          withAction:UPDATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
}


@end

