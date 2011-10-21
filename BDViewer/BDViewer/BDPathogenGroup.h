//
//  BDPathogenGroup.h
//  BDViewer
//
//  Created by Liz Dovey on 11-10-05.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

#define SCHEMAVERSION_PATHOGENGROUP @"1"
#define DOMAIN_PATHOGENGROUP @"bd_1_pathogenGroups"
#define BUCKET_PATHOGENGROUP @"bdDataStore"
#define ENTITYNAME_PATHOGENGROUP @"BDPathogenGroup"

#define PG_UUID @"pg_uuid"
#define PG_SCHEMAVERSION @"pg_schemaVersion"
#define PG_CREATEDDATE @"pg_createdDate"
#define PG_CREATEDBY @"pg_createdBy"
#define PG_MODIFIEDDATE @"pg_modifiedDate"
#define PG_MODIFIEDBY @"pg_modifiedBy"
#define PG_DEPRECATED @"pg_deprecated"
#define PG_INUSEBY @"pg_inUseBy"
#define PG_DISPLAYORDER @"pg_displayOrder"
#define PG_PRESENTATIONID @"pg_presentationId"

@interface BDPathogenGroup : NSManagedObject {
@private
}
@property (nonatomic, retain) NSString * uuid;
@property (nonatomic, retain) NSString * createdBy;
@property (nonatomic, retain) NSDate * createdDate;
@property (nonatomic, retain) NSNumber * deprecated;
@property (nonatomic, retain) NSString * inUseBy;
@property (nonatomic, retain) NSString * modifiedBy;
@property (nonatomic, retain) NSDate * modifiedDate;
@property (nonatomic, retain) NSNumber * schemaVersion;
@property (nonatomic, retain) NSNumber * displayOrder;
@property (nonatomic, retain) NSString * presentationId;

+(NSString *)create;
+(BDPathogenGroup *)retrieveWithUUID:(NSString *)theUUID;
+(NSArray *) retrieveAllWithParentUUID:(NSString *)theUUID;
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer; 

-(void)commitChanges;
@end
