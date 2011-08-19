//
//  EditorDocument.h
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-18.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>


@interface EditorDocument : NSManagedObject

@property (nonatomic, retain) NSString * uuid;
@property (nonatomic, retain) NSDate * createdDate;
@property (nonatomic, retain) NSDate * modifiedDate;
@property (nonatomic, retain) NSString * documentText;
@property (nonatomic, retain) NSString * createdBy;
@property (nonatomic, retain) NSString * lastModifiedBy;
@property (nonatomic, retain) NSString * inUseBy;

+(EditorDocument *)create;
+(EditorDocument *)retrieveWithUUID:(NSString *)theUUID;
@end
