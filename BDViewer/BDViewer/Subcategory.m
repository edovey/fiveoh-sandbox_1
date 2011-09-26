//
//  Subcategory.m
//  BDViewer
//
//  Created by GrubbyHedgehog on 11-09-19.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "Subcategory.h"
#import "NSString+UUID.h"
#import "QueueEntry.h"


@implementation Subcategory

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

+(NSString *)create
{
    NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
    NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_SUBCATEGORY inManagedObjectContext:moc];
    
	Subcategory *subcategory = [[Subcategory alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
    
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
    

    [QueueEntry createWithObjectUuid:subcategory.uuid 
                      withEntityName:ENTITYNAME_SUBCATEGORY 
                          withAction:CREATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
    NSString *uuid = subcategory.uuid;
    
    return uuid;
}

+(Subcategory *)retrieveWithUUID:(NSString *)theUUID
{
    return (Subcategory *)[[DataController sharedInstance] retrieveManagedObject:ENTITYNAME_SUBCATEGORY 
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
    
    Subcategory *subcategory = [Subcategory retrieveWithUUID:uuid];
    if(nil == subcategory)
    {
        NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
        NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_SUBCATEGORY 
                                                  inManagedObjectContext:moc];
        
        subcategory = [[Subcategory alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
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
                      withEntityName:ENTITYNAME_SUBCATEGORY 
                          withAction:UPDATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
}


@end

