//
//  BDLinkedNoteAssociation.h
//  BDViewer
//
//  Created by Liz Dovey on 11-10-14.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>


@interface BDLinkedNoteAssociation : NSManagedObject

@property (nonatomic, retain) NSString * uuid;
@property (nonatomic, retain) NSDate * createdBy;
@property (nonatomic, retain) NSDate * createdDate;
@property (nonatomic, retain) NSString * modifiedBy;
@property (nonatomic, retain) NSDate * modifiedDate;
@property (nonatomic, retain) NSNumber * schemaVersion;
@property (nonatomic, retain) NSNumber * deprecated;
@property (nonatomic, retain) NSNumber * displayOrder;
@property (nonatomic, retain) NSString * linkedNoteId;
@property (nonatomic, retain) NSString * parentId;
@property (nonatomic, retain) NSString * parentEntityName;
@property (nonatomic, retain) NSString * parentEntiyPropertyName;
@property (nonatomic, retain) NSNumber * linkedNoteType;

@end
