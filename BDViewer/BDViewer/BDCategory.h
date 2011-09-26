//
//  Category.h
//  BDViewer
//
//  Created by GrubbyHedgehog on 11-09-19.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

#define SCHEMAVERSION_CATEGORY @"1"
#define DOMAIN_CATEGORY @"bd_test2"
#define BUCKET_CATEGORY @"bdDataStore"
#define ENTITYNAME_CATEGORY @"BDCategory"

#define CT_UUID @"ct_uuid"
#define CT_SCHEMAVERSION @"ct_schemaVersion"
#define CT_CREATEDDATE @"ct_createdDate"
#define CT_CREATEDBY @"ct_createdBy"
#define CT_MODIFIEDDATE @"ct_modifiedDate"
#define CT_MODIFIEDBY @"ct_modifiedBy"
#define CT_STORAGEKEY @"ct_storageKey"
#define CT_DEPRECATED @"ct_deprecated"
#define CT_INUSEBY @"ct_inUseBy"
#define CT_SECTIONID @"ct_sectionId"
#define CT_NAME @"ct_name"

@interface BDCategory : NSManagedObject

@property (nonatomic, retain) NSString * uuid;
@property (nonatomic, retain) NSString * sectionId;
@property (nonatomic, retain) NSString * name;
@property (nonatomic, retain) NSString * createdBy;
@property (nonatomic, retain) NSDate * createdDate;
@property (nonatomic, retain) NSNumber * deprecated;
@property (nonatomic, retain) NSString * inUseBy;
@property (nonatomic, retain) NSString * modifiedBy;
@property (nonatomic, retain) NSDate * modifiedDate;
@property (nonatomic, retain) NSNumber * schemaVersion;

+(NSString *)create;
+(BDCategory *)retrieveWithUUID:(NSString *)theUUID;
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer; 

-(void)commitChanges;
@end
