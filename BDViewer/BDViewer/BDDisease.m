//
//  Disease.m
//  BDViewer
//
//  Created by Liz Dovey on 11-09-26.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "BDDisease.h"
#import "NSString+UUID.h"
#import "BDQueueEntry.h"

@implementation BDDisease
@dynamic createdBy;
@dynamic createdDate;
@dynamic deprecated;
@dynamic inUseBy;
@dynamic modifiedBy;
@dynamic modifiedDate;
@dynamic name;
@dynamic displayOrder;
@dynamic schemaVersion;
@dynamic subcategoryId;
@dynamic uuid;
@dynamic categoryId;

+(NSString *)create
{
    NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
    NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_DISEASE inManagedObjectContext:moc];
    
	BDDisease *disease = [[BDDisease alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
    
    disease.uuid = [NSString UUIDCreate];
    disease.createdDate = [NSDate date];
    disease.createdBy = [[UIDevice currentDevice] uniqueIdentifier];
    disease.modifiedDate = disease.createdDate;
    disease.modifiedBy = [[UIDevice currentDevice] uniqueIdentifier];
    disease.inUseBy = nil;
    disease.schemaVersion = [NSNumber numberWithInt:[SCHEMAVERSION_DISEASE intValue]];
    disease.deprecated = [NSNumber numberWithBool:NO];
    disease.displayOrder = [NSNumber numberWithInt:-1];
    
    [BDQueueEntry createWithObjectUuid:disease.uuid 
                      withEntityName:ENTITYNAME_DISEASE 
                          withAction:CREATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
    NSString *uuid = disease.uuid;
    [disease release];
    
    return uuid;
}

+(BDDisease *)retrieveWithUUID:(NSString *)theUUID
{
    return (BDDisease *)[[DataController sharedInstance] retrieveManagedObject:ENTITYNAME_DISEASE 
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
    
    NSString *uuid = [theAttributeDictionary valueForKey:DI_UUID];
    NSDate *modifedDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:DI_MODIFIEDDATE]];
    
    BDDisease *disease = [BDDisease retrieveWithUUID:uuid];
    if(nil == disease)
    {
        NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
        NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_DISEASE 
                                                  inManagedObjectContext:moc];
        
        disease = [[[BDDisease alloc] initWithEntity:entity insertIntoManagedObjectContext:moc] autorelease];
        disease.uuid = uuid;
        
    }
    
    NSLog(@"Local Disease Modified Date:%@", [dateFormatter stringFromDate:disease.modifiedDate]);
    NSLog(@"Repository Disease Modified Date:%@", [dateFormatter stringFromDate:modifedDate]);
    
    if ( (nil == disease.modifiedDate) 
        || ( ([disease.modifiedDate compare:modifedDate] == NSOrderedAscending) || (overwriteNewer) ) )
    {
        int schemaVersion = [[theAttributeDictionary valueForKey:DI_SCHEMAVERSION] intValue];
        
        disease.schemaVersion = [NSNumber numberWithInt:schemaVersion];
        
        switch (schemaVersion)
        {
            case 1:
            default:
            {
                disease.createdBy = [theAttributeDictionary valueForKey:DI_CREATEDBY];
                disease.createdDate = [dateFormatter dateFromString:[theAttributeDictionary valueForKey:DI_CREATEDDATE]];
                disease.modifiedBy  = [theAttributeDictionary valueForKey:DI_MODIFIEDBY];
                disease.modifiedDate = modifedDate;
                disease.inUseBy = [theAttributeDictionary valueForKey:DI_INUSEBY];
                disease.deprecated = [NSNumber numberWithBool:[[theAttributeDictionary valueForKey:DI_DEPRECATED] boolValue]];
                
                disease.subcategoryId = [theAttributeDictionary valueForKey:DI_SUBCATEGORYID];
                disease.categoryId = [theAttributeDictionary valueForKey:DI_CATEGORYID];
                disease.name = [theAttributeDictionary valueForKey:DI_NAME];
                disease.displayOrder = [NSNumber numberWithInt:[[theAttributeDictionary valueForKey:DI_DISPLAYORDER] intValue]];
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
                      withEntityName:ENTITYNAME_DISEASE 
                          withAction:UPDATE_QueueEntryActionType 
                            withSave:NO];
    
    [[DataController sharedInstance] saveContext];
}


@end
