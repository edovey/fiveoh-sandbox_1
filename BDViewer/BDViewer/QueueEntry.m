//
//  QueueEntry.m
//  BDViewer
//
//  Created by Karl Schulze on 2011-08-24.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "QueueEntry.h"

@implementation QueueEntry

@dynamic uuid;
@dynamic timestamp;
@dynamic objectUuid;
@dynamic objectEntityName;
@dynamic action;

+(void) createWithObjectUuid:(NSString *)theUuid 
              withEntityName:(NSString *)theEntityName 
                  withAction:(QueueEntryActionType)theActionType 
                    withSave:(BOOL)save
{
    QueueEntry *existingEntry = [QueueEntry retrieveForObjectUuid:theUuid];
    if(nil == existingEntry)
    {
        NSManagedObjectContext *moc = [[DataController sharedInstance] managedObjectContext]; 
        NSEntityDescription *entity = [NSEntityDescription entityForName:ENTITYNAME_QUEUEENTRY inManagedObjectContext:moc];
        
        QueueEntry *entry = [[QueueEntry alloc] initWithEntity:entity insertIntoManagedObjectContext:moc];
        entry.uuid = [NSString UUIDCreate];
        entry.timestamp = [NSDate date];
        entry.objectUuid = theUuid;
        entry.objectEntityName = theEntityName;
        entry.action = [NSNumber numberWithInt:theActionType];
        if(save) [[DataController sharedInstance] saveContext];
    }
}

+(QueueEntry *)retrieveForObjectUuid:(NSString *)theUuid
{    
    return (QueueEntry *)[[DataController sharedInstance] retrieveManagedObjectForValue:ENTITYNAME_QUEUEENTRY withKey:@"objectUuid" withValue:theUuid withMOC:nil];
}

@end
