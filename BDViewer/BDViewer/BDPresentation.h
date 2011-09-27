//
//  Presentation.h
//  BDViewer
//
//  Created by Liz Dovey on 11-09-26.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

#define SCHEMAVERSION_PRESENTATION @"1"
#define DOMAIN_PRESENTATION @"bd_test2"
#define BUCKET_PRESENTATION @"bdDataStore"
#define ENTITYNAME_PRESENTATION @"BDPresentation"

#define PR_UUID @"pr_uuid"
#define PR_SCHEMAVERSION @"pr_schemaVersion"
#define PR_CREATEDDATE @"pr_createdDate"
#define PR_CREATEDBY @"pr_createdBy"
#define PR_MODIFIEDDATE @"ln_modifiedDate"
#define PR_MODIFIEDBY @"pr_modifiedBy"
#define PR_STORAGEKEY @"pr_storageKey"
#define PR_DEPRECATED @"pr_deprecated"
#define PR_INUSEBY @"pr_inUseBy"
#define PR_DISEASEID @"pr_diseaseId"
#define PR_OVERVIEW @"pr_overview"
#define PR_DISPLAYORDER @"pr_displayOrder"
#define PR_NAME @"pr_name"


@interface BDPresentation : NSManagedObject {
@private
}
@property (nonatomic, retain) NSString * createdBy;
@property (nonatomic, retain) NSDate * createdDate;
@property (nonatomic, retain) NSNumber * deprecated;
@property (nonatomic, retain) NSString * diseaseId;
@property (nonatomic, retain) NSString * inUseBy;
@property (nonatomic, retain) NSString * modifiedBy;
@property (nonatomic, retain) NSDate * modifiedDate;
@property (nonatomic, retain) NSString * name;
@property (nonatomic, retain) NSString * overview;
@property (nonatomic, retain) NSNumber * displayOrder;
@property (nonatomic, retain) NSNumber * schemaVersion;
@property (nonatomic, retain) NSString * uuid;

+(NSString *)create;
+(BDPresentation *)retrieveWithUUID:(NSString *)theUUID;
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer; 

-(void)commitChanges;
@end
