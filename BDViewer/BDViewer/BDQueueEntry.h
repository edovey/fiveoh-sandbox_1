//
//  QueueEntry.h
//  BDViewer
//
//  Created by Karl Schulze on 2011-08-24.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

#define ENTITYNAME_QUEUEENTRY @"BDQueueEntry"

typedef enum
{
    UPDATE_QueueEntryActionType = 0,
    CREATE_QueueEntryActionType = 1,
    DEPRECATE_QueueEntryActionType = 2
} QueueEntryActionType;

@interface BDQueueEntry : NSManagedObject

@property (nonatomic, retain) NSString      * uuid;
@property (nonatomic, retain) NSDate        * timestamp;
@property (nonatomic, retain) NSString      * objectUuid;
@property (nonatomic, retain) NSString      * objectEntityName;
@property (nonatomic, retain) NSNumber      * action;

+(void)createWithObjectUuid:(NSString *)theUuid 
             withEntityName:(NSString *)theEntityName 
                 withAction:(QueueEntryActionType)theActionType 
                   withSave:(BOOL)save;

+(BDQueueEntry *)retrieveForObjectUuid:(NSString *)theUuid;

@end
