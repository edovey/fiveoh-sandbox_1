//
//  BDChapter.h
//  BDViewer
//
//  Created by Liz Dovey on 11-10-18.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

#define SCHEMAVERSION_CHAPTER @"1"
#define DOMAIN_CHAPTER @"bd_1_chapters"
#define BUCKET_CHAPTER @"bdDataStore"
#define ENTITYNAME_CHAPTER @"BDChapter"

#define CH_UUID @"ch_uuid"
#define CH_SCHEMAVERSION @"ch_schemaVersion"
#define CH_CREATEDDATE @"ch_createdDate"
#define CH_CREATEDBY @"ch_createdBy"
#define CH_MODIFIEDDATE @"ch_modifiedDate"
#define CH_MODIFIEDBY @"ch_modifiedBy"
#define CH_DEPRECATED @"ch_deprecated"
#define CH_INUSEBY @"ch_inUseBy"
#define CH_DISPLAYORDER @"ch_displayOrder"
#define CH_NAME @"ch_name"


@interface BDChapter : NSManagedObject

@property (nonatomic, retain) NSString * createdBy;
@property (nonatomic, retain) NSDate * createdDate;
@property (nonatomic, retain) NSNumber * deprecated;
@property (nonatomic, retain) NSNumber * displayOrder;
@property (nonatomic, retain) NSString * inUseBy;
@property (nonatomic, retain) NSString * modifiedBy;
@property (nonatomic, retain) NSDate * modifiedDate;
@property (nonatomic, retain) NSString * name;
@property (nonatomic, retain) NSNumber * schemaVersion;
@property (nonatomic, retain) NSString * uuid;

+(NSString *)create;
+(BDChapter *)retrieveWithUUID:(NSString *)theUUID;
+(NSArray *)retrieveAll;
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer; 

-(void)commitChanges;

@end
