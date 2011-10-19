//
//  Disease.h
//  BDViewer
//
//  Created by Liz Dovey on 11-09-26.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

#define SCHEMAVERSION_DISEASE @"1"
#define DOMAIN_DISEASE @"bd_diseases"
#define BUCKET_DISEASE @"bdDataStore"
#define ENTITYNAME_DISEASE @"BDDisease"

#define DI_UUID @"di_uuid"
#define DI_SCHEMAVERSION @"di_schemaVersion"
#define DI_CREATEDDATE @"di_createdDate"
#define DI_CREATEDBY @"di_createdBy"
#define DI_MODIFIEDDATE @"di_modifiedDate"
#define DI_MODIFIEDBY @"di_modifiedBy"
#define DI_DEPRECATED @"di_deprecated"
#define DI_INUSEBY @"di_inUseBy"
#define DI_DISPLAYORDER @"di_displayOrder"
#define DI_SUBCATEGORYID @"di_subcategoryId"
#define DI_CATEGORYID @ "di_categoryId"
#define DI_NAME @"di_name"


@interface BDDisease : NSManagedObject {
@private
}
@property (nonatomic, retain) NSString * createdBy;
@property (nonatomic, retain) NSDate * createdDate;
@property (nonatomic, retain) NSNumber * deprecated;
@property (nonatomic, retain) NSString * inUseBy;
@property (nonatomic, retain) NSString * modifiedBy;
@property (nonatomic, retain) NSDate * modifiedDate;
@property (nonatomic, retain) NSString * name;
@property (nonatomic, retain) NSNumber * schemaVersion;
@property (nonatomic, retain) NSString * subcategoryId;
@property (nonatomic, retain) NSString * uuid;
@property (nonatomic, retain) NSString * categoryId;
@property (nonatomic, retain) NSNumber * displayOrder;

+(NSString *) create;
+(BDDisease *) retrieveWithUUID:(NSString *)theUUID;
+(NSArray *)retrieveAll;
+(NSString *) loadWithAttributes:(NSDictionary *)theAttributeDictionary withOverwriteNewerFlag:(BOOL)overwriteNewer;

-(void)commitChanges;

@end
