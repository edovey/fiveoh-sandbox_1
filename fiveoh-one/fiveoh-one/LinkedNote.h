//
//  EditorDocument.h
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-18.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

#define SCHEMAVERSION_LINKEDNOTE @"1"
#define DOMAIN_LINKEDNOTE @"bd_test2"
#define BUCKET_LINKEDNOTE @"bdDataStore"
#define ENTITYNAME_LINKEDNOTE @"LinkedNote"

#define LN_UUID @"ln_uuid"
#define LN_SCHEMAVERSION @"ln_schemaVersion"
#define LN_CREATEDDATE @"ln_createdDate"
#define LN_CREATEDBY @"ln_createdBy"
#define LN_MODIFIEDDATE @"ln_modifiedDate"
#define LN_MODIFIEDBY @"ln_modifiedBy"
#define LN_STORAGEKEY @"ln_storageKey"
#define LN_DEPRECATED @"ln_deprecated"
#define LN_INUSEBY @"ln_inUseBy"
#define LN_PARENTID @"ln_parentId"

#define S3_LN_DOCUMENTTEXT @"ln_documentText"

@interface LinkedNote : NSManagedObject

@property (nonatomic, retain) NSString  * uuid;
@property (nonatomic, retain) NSDate    * createdDate;
@property (nonatomic, retain) NSDate    * modifiedDate;
@property (nonatomic, retain) NSString  * documentText; // Stored in S3 as refeferenced by storageKey
@property (nonatomic, retain) NSString  * createdBy;
@property (nonatomic, retain) NSString  * modifiedBy;
@property (nonatomic, retain) NSString  * inUseBy;
@property (nonatomic, retain) NSString  * storageKey;
@property (nonatomic, retain) NSNumber  * deprecated;
@property (nonatomic, retain) NSNumber  * schemaVersion;


+(NSString *)create;
+(LinkedNote *)retrieveWithUUID:(NSString *)theUUID;
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer; 

-(void)commitChanges;
@end
