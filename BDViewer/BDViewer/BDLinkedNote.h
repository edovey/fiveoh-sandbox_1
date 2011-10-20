//
//  EditorDocument.h
//  BDViewer
//
//  Created by Karl Schulze on 2011-08-18.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>
#import "RepositoryProtocol.h"

#define SCHEMAVERSION_LINKEDNOTE @"1"
#define DOMAIN_LINKEDNOTE @"bd_linkedNotes"
#define BUCKET_LINKEDNOTE @"bdDataStore"
#define ENTITYNAME_LINKEDNOTE @"BDLinkedNote"

#define AWS_S3_PREFIX_LINKEDNOTE  @"bd~";
#define AWS_S3_FILEEXTENSION_LINKEDNOTE  @".txt";

#define LN_UUID @"ln_uuid"
#define LN_CREATEDDATE @"ln_createdDate"
#define LN_CREATEDBY @"ln_createdBy"
#define LN_MODIFIEDDATE @"ln_modifiedDate"
#define LN_MODIFIEDBY @"ln_modifiedBy"
#define LN_INUSEBY @"ln_inUseBy"
#define LN_DEPRECATED @"ln_deprecated"
#define LN_SCHEMAVERSION @"ln_schemaVersion"
#define LN_DISPLAYORDER @"ln_displayOrder"
#define LN_LINKEDNOTEASSOCIATIONID @"ln_linkedNoteAssociationId"
#define LN_PREVIEWTEXT @"ln_previewText"
#define LN_SCOPEID @"ln_scopeId"
#define LN_SINGLEUSE @"ln_singelUse"
#define LN_STORAGEKEY @"ln_storageKey"
#define LN_DOCUMENTTEXT @"ln_documentText"

#define S3_LN_DOCUMENTTEXT @"ln_documentText"

@interface BDLinkedNote : NSManagedObject <RepositoryProtocol>

@property (nonatomic, retain) NSString  * uuid;
@property (nonatomic, retain) NSDate    * createdDate;
@property (nonatomic, retain) NSString  * createdBy;
@property (nonatomic, retain) NSString  * modifiedBy;
@property (nonatomic, retain) NSDate    * modifiedDate;
@property (nonatomic, retain) NSString  * inUseBy;
@property (nonatomic, retain) NSNumber  * deprecated;
@property (nonatomic, retain) NSNumber  * schemaVersion;
@property (nonatomic, retain) NSNumber * displayOrder;

@property (nonatomic, retain) NSString * linkedNoteAssociationId;
@property (nonatomic, retain) NSString * previewText;
@property (nonatomic, retain) NSString * scopeId;
@property (nonatomic, retain) NSNumber * singleUse;
@property (nonatomic, retain) NSString  * storageKey;
@property (nonatomic, retain) NSString  * documentText; // Stored in S3 as refeferenced by storageKey


+(NSString *)create;
+(BDLinkedNote *)retrieveWithUUID:(NSString *)theUUID;

//+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
//         withOverwriteNewerFlag:(BOOL)overwriteNewer; 

-(void)commitChanges;
@end
