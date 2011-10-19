//
//  Therapy.h
//  BDViewer
//
//  Created by Liz Dovey on 11-09-26.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

#define SCHEMAVERSION_THERAPY @"1"
#define DOMAIN_THERAPY @"bd_therapies"
#define BUCKET_THERAPY @"bdDataStore"
#define ENTITYNAME_THERAPY @"BDTherapy"

#define TH_UUID @"th_uuid"
#define TH_SCHEMAVERSION @"th_schemaVersion"
#define TH_CREATEDDATE @"th_createdDate"
#define TH_CREATEDBY @"th_createdBy"
#define TH_INUSEBY @"th_inUseBy"
#define TH_MODIFIEDDATE @"th_modifiedDate"
#define TH_MODIFIEDBY @"th_modifiedBy"
#define TH_STORAGEKEY @"th_storageKey"
#define TH_DEPRECATED @"th_deprecated"
#define TH_INUSEBY @"th_inUseBy"
#define TH_THERAPYGROUPID @"th_therapyGroupId"
#define TH_THERAPYGROUPJOINTYPE @"th_therapyGroupJoinType"
#define TH_DISPLAYORDER @"th_displayOrder"
#define TH_DOSAGE @"th_dosage"
#define TH_DURATION @"th_duration"
#define TH_NAME @"th_name"

@interface BDTherapy : NSManagedObject {
@private
}
@property (nonatomic, retain) NSString * createdBy;
@property (nonatomic, retain) NSDate * createdDate;
@property (nonatomic, retain) NSNumber * deprecated;
@property (nonatomic, retain) NSNumber * displayOrder;
@property (nonatomic, retain) NSString * dosage;
@property (nonatomic, retain) NSString * duration;
@property (nonatomic, retain) NSString * inUseBy;
@property (nonatomic, retain) NSString * modifiedBy;
@property (nonatomic, retain) NSDate * modifiedDate;
@property (nonatomic, retain) NSString * name;
@property (nonatomic, retain) NSNumber * schemaVersion;
@property (nonatomic, retain) NSString * therapyGroupId;
@property (nonatomic, retain) NSNumber * therapyGroupJoinType;
@property (nonatomic, retain) NSString * uuid;

+(NSString *)create;
+(BDTherapy *)retrieveWithUUID:(NSString *)theUUID;
+(NSArray *) retrieveAllWithParentUUID:(NSString *)theUUID parentPropertyName:(NSString *)thePropertyName;
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer; 

-(void)commitChanges;
@end
