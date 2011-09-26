//
//  Therapy.h
//  BDViewer
//
//  Created by Liz Dovey on 11-09-26.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>


@interface Therapy : NSManagedObject {
@private
}
@property (nonatomic, retain) NSString * createdBy;
@property (nonatomic, retain) NSDate * createdDate;
@property (nonatomic, retain) NSNumber * deprecated;
@property (nonatomic, retain) NSString * dosage;
@property (nonatomic, retain) NSString * duration;
@property (nonatomic, retain) NSString * modifiedBy;
@property (nonatomic, retain) NSDate * modifiedDate;
@property (nonatomic, retain) NSString * name;
@property (nonatomic, retain) NSNumber * schemaVersion;
@property (nonatomic, retain) NSString * therapyGroupId;
@property (nonatomic, retain) NSNumber * therapyGroupJoinType;
@property (nonatomic, retain) NSNumber * therapyDisplayOrder;
@property (nonatomic, retain) NSString * uuid;

@end
