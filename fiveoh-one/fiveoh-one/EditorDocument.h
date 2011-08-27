//
//  EditorDocument.h
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-18.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

#define SCHEMAVERSION_EDITORDOCUMENT @"1"
#define DOMAIN_EDITORDOCUMENT @"bd_test2"
#define BUCKET_EDITORDOCUMENT @"bdDataStore"
#define ENTITYNAME_EDITORDOCUMENT @"EditorDocument"

#define ED_UUID @"ed_uuid"
#define ED_SCHEMAVERSION @"ed_schemaVersion"
#define ED_CREATEDDATE @"ed_createdDate"
#define ED_CREATEDBY @"ed_createdBy"
#define ED_MODIFIEDDATE @"ed_modifiedDate"
#define ED_MODIFIEDBY @"ed_modifiedBy"
#define ED_STORAGEKEY @"ed_storageKey"
#define ED_DEPRECATED @"ed_deprecated"
#define ED_INUSEBY @"ed_inUseBy"

#define S3_ED_DOCUMENTTEXT @"ed_documentText"

@interface EditorDocument : NSManagedObject

@property (nonatomic, retain) NSString  * uuid;
@property (nonatomic, retain) NSDate    * createdDate;
@property (nonatomic, retain) NSDate    * modifiedDate;
@property (nonatomic, retain) NSString  * documentText; // Stored in S3 as refeferenced by storageURL
@property (nonatomic, retain) NSString  * createdBy;
@property (nonatomic, retain) NSString  * modifiedBy;
@property (nonatomic, retain) NSString  * inUseBy;
@property (nonatomic, retain) NSString  * storageKey;
@property (nonatomic, retain) NSNumber  * deprecated;
@property (nonatomic, retain) NSNumber  * schemaVersion;


+(NSString *)create;
+(EditorDocument *)retrieveWithUUID:(NSString *)theUUID;
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer; 

-(void)commitChanges;
@end
