//
//  Section.h
//  BDViewer
//
//  Created by GrubbyHedgehog on 11-09-19.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

#define SCHEMAVERSION_SECTION @"1"
#define DOMAIN_SECTION @"bd_1_sections"
#define BUCKET_SECTION @"bdDataStore"
#define ENTITYNAME_SECTION @"BDSection"

#define SN_UUID @"sn_uuid"
#define SN_SCHEMAVERSION @"sn_schemaVersion"
#define SN_CREATEDDATE @"sn_createdDate"
#define SN_CREATEDBY @"sn_createdBy"
#define SN_MODIFIEDDATE @"sn_modifiedDate"
#define SN_MODIFIEDBY @"sn_modifiedBy"
#define SN_DEPRECATED @"sn_deprecated"
#define SN_INUSEBY @"sn_inUseBy"
#define SN_CHAPTERID @"sn_chapterId"
#define SN_DISPLAYORDER @"sn_displayOrder"
#define SN_NAME @"sn_name"

@interface BDSection : NSManagedObject

@property (nonatomic, retain) NSString * uuid;
@property (nonatomic, retain) NSString * name;
@property (nonatomic, retain) NSString * createdBy;
@property (nonatomic, retain) NSDate * createdDate;
@property (nonatomic, retain) NSNumber * deprecated;
@property (nonatomic, retain) NSString * inUseBy;
@property (nonatomic, retain) NSString * modifiedBy;
@property (nonatomic, retain) NSDate * modifiedDate;
@property (nonatomic, retain) NSNumber * schemaVersion;
@property (nonatomic, retain) NSNumber * displayOrder;
@property (nonatomic, retain) NSString * chapterId;

+(NSString *)create;
+(BDSection *)retrieveWithUUID:(NSString *)theUUID;
+(NSArray *) retrieveAll;
+(NSArray *) retrieveAllWithParentUUID:(NSString *)theUUID;
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer; 
+(NSNumber *) retrieveCountWithParentUUID:(NSString *)theUUID;

-(void)commitChanges;
@end

