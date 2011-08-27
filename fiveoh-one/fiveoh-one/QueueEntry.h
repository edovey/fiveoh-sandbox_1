//
//  QueueEntry.h
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-24.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

#define ENTITYNAME_QUEUEENTRY @"QueueEntry"

typedef enum
{
    UPDATE_QueueEntryActionType = 0,
    CREATE_QueueEntryActionType = 1,
    DEPRECATE_QueueEntryActionType = 2
} QueueEntryActionType;

@interface QueueEntry : NSManagedObject

@property (nonatomic, retain) NSString      * uuid;
@property (nonatomic, retain) NSDate        * timestamp;
@property (nonatomic, retain) NSString      * objectUuid;
@property (nonatomic, retain) NSString      * objectEntityName;
@property (nonatomic, retain) NSNumber      * action;

+(void)createWithObjectUuid:(NSString *)theUuid 
             withEntityName:(NSString *)theEntityName 
                 withAction:(QueueEntryActionType)theActionType 
                   withSave:(BOOL)save;

+(QueueEntry *)retrieveForObjectUuid:(NSString *)theUuid;

@end
