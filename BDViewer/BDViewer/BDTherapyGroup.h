//
//  TherapyGroup.h
//  BDViewer
//
//  Created by Liz Dovey on 11-09-26.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

#define SCHEMAVERSION_THERAPYGROUP @"1"
#define DOMAIN_THERAPYGROUP @"bd_therapyGroups"
#define BUCKET_THERAPYGROUP @"bdDataStore"
#define ENTITYNAME_THERAPYGROUP @"BDTherapyGroup"

#define TG_UUID @"tg_uuid"
#define TG_SCHEMAVERSION @"tg_schemaVersion"
#define TG_CREATEDDATE @"tg_createdDate"
#define TG_CREATEDBY @"tg_createdBy"
#define TG_MODIFIEDDATE @"tg_modifiedDate"
#define TG_MODIFIEDBY @"tg_modifiedBy"
#define TG_STORAGEKEY @"tg_storageKey"
#define TG_DEPRECATED @"tg_deprecated"
#define TG_INUSEBY @"tg_inUseBy"
#define TG_PATHOGENGROUPID @"tg_pathogenGroupId"
#define TG_NAME @"tg_name"
#define TG_DISPLAYORDER @"tg_displayOrder"

@interface BDTherapyGroup : NSManagedObject {
@private
}
@property (nonatomic, retain) NSString * createdBy;
@property (nonatomic, retain) NSDate * createdDate;
@property (nonatomic, retain) NSNumber * deprecated;
@property (nonatomic, retain) NSNumber * displayOrder;
@property (nonatomic, retain) NSString * inUseBy;
@property (nonatomic, retain) NSString * modifiedBy;
@property (nonatomic, retain) NSDate * modifiedDate;
@property (nonatomic, retain) NSString * name;
@property (nonatomic, retain) NSString * pathogenGroupId;
@property (nonatomic, retain) NSNumber * schemaVersion;
@property (nonatomic, retain) NSString * uuid;

+(NSString *)create;
+(BDTherapyGroup *)retrieveWithUUID:(NSString *)theUUID;
+(NSArray *) retrieveAllWithParentUUID:(NSString *)theUUID;
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer; 

-(void)commitChanges;
@end
