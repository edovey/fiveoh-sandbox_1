//
//  Bd_editorDocument.h
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-24.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>


@interface Bd_editorDocument : NSManagedObject

@property (nonatomic, retain) NSString * itemName;
@property (nonatomic, retain) NSDate * modifiedDate;
@property (nonatomic, retain) NSString * inUseBy;

@end
