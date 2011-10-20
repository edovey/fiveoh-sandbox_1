//  Subcategory.h
//  BDViewer
//
//
//  Created by GrubbyHedgehog on 11-09-19.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>


#define SCHEMAVERSION_SUBCATEGORY @"1"
#define DOMAIN_SUBCATEGORY @"bd_subcategories"
#define BUCKET_SUBCATEGORY @"bdDataStore"
#define ENTITYNAME_SUBCATEGORY @"BDSubcategory"

#define SC_UUID @"sc_uuid"
#define SC_SCHEMAVERSION @"sc_schemaVersion"
#define SC_CREATEDDATE @"sc_createdDate"
#define SC_CREATEDBY @"sc_createdBy"
#define SC_MODIFIEDDATE @"sc_modifiedDate"
#define SC_MODIFIEDBY @"sc_modifiedBy"
#define SC_STORAGEKEY @"sc_storageKey"
#define SC_DEPRECATED @"sc_deprecated"
#define SC_INUSEBY @"sc_inUseBy"
#define SC_DISPLAYORDER @"sc_displayOrder"
#define SC_CATEGORYID @"sc_categoryId"
#define SC_NAME @"sc_name"

@interface BDSubcategory : NSManagedObject

@property (nonatomic, retain) NSString * uuid;
@property (nonatomic, retain) NSString * name;
@property (nonatomic, retain) NSString * categoryId;
@property (nonatomic, retain) NSString * createdBy;
@property (nonatomic, retain) NSDate * createdDate;
@property (nonatomic, retain) NSNumber * deprecated;
@property (nonatomic, retain) NSString * inUseBy;
@property (nonatomic, retain) NSString * modifiedBy;
@property (nonatomic, retain) NSDate * modifiedDate;
@property (nonatomic, retain) NSNumber * schemaVersion;
@property (nonatomic, retain) NSNumber * displayOrder;

+(NSString *)create;
+(BDSubcategory *)retrieveWithUUID:(NSString *)theUUID;
+(NSArray *) retrieveAllWithParentUUID:(NSString *)theUUID;
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer; 

-(void)commitChanges;
@end
