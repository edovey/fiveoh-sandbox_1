//
//  Pathogen.h
//  BDViewer
//
//  Created by Liz Dovey on 11-09-26.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

#define SCHEMAVERSION_PATHOGEN @"1"
#define DOMAIN_PATHOGEN @"bd_test2"
#define BUCKET_PATHOGEN @"bdDataStore"
#define ENTITYNAME_PATHOGEN @"BDPathogen"

#define PA_UUID @"pa_uuid"
#define PA_SCHEMAVERSION @"pa_schemaVersion"
#define PA_CREATEDDATE @"pa_createdDate"
#define PA_CREATEDBY @"pa_createdBy"
#define PA_MODIFIEDDATE @"pa_modifiedDate"
#define PA_MODIFIEDBY @"pa_modifiedBy"
#define PA_DEPRECATED @"pa_deprecated"
#define PA_INUSEBY @"pa_inUseBy"
#define PA_DISPLAYORDER @"pa_displayOrder"
#define PA_PATHOGENGROUPID @"pa_pathogenGroupId"
#define PA_NAME @"pa_name"

@interface BDPathogen : NSManagedObject {
@private
}
@property (nonatomic, retain) NSString * createdBy;
@property (nonatomic, retain) NSDate * createdDate;
@property (nonatomic, retain) NSNumber * deprecated;
@property (nonatomic, retain) NSString * inUseBy;
@property (nonatomic, retain) NSString * modifiedBy;
@property (nonatomic, retain) NSNumber * displayOrder;
@property (nonatomic, retain) NSDate * modifiedDate;
@property (nonatomic, retain) NSString * name;
@property (nonatomic, retain) NSString * pathogenGroupId;
@property (nonatomic, retain) NSNumber * schemaVersion;
@property (nonatomic, retain) NSString * uuid;

+(NSString *)create;
+(BDPathogen *)retrieveWithUUID:(NSString *)theUUID;
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer; 

-(void)commitChanges;
@end
