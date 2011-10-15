//
//  BDLinkedNoteAssociation.h
//  BDViewer
//
//  Created by Liz Dovey on 11-10-14.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

<<<<<<< HEAD
#define SCHEMAVERSION_LINKEDNOTEASSOCIATION @"1"
#define DOMAIN_LINKEDNOTEASSOCIATION @"bd_test2"
#define BUCKET_LINKEDNOTEASSOCIATION @"bdDataStore"
#define ENTITYNAME_LINKEDNOTEASSOCIATION @"BDLinkedNoteAssociation"

#define LA_UUID @"la_uuid"
#define LA_CREATEDDATE @"la_createdDate"
#define LA_CREATEDBY @"la_createdBy"
#define LA_MODIFIEDDATE @"la_modifiedDate"
#define LA_MODIFIEDBY @"la_modifiedBy"
#define LA_DEPRECATED @"la_deprecated"
#define LA_SCHEMAVERSION @"la_schemaVersion"
#define LA_DISPLAYORDER @"la_displayOrder"
#define LA_LINKEDNOTEID @"la_linkedNoteId"
#define LA_PARENTID @"la_parentId"
#define LA_PARENTENTITYNAME @"la_parentEntityName"
#define LA_PARENTENTITYPROPERTYNAME @"la_parentEntityPropertyName"
#define LA_LINKEDNOTETYPE @"la_linkedNoteType"
=======
>>>>>>> 2ba7e82d91d3dd1edb435395d6dda9ff888fe580

@interface BDLinkedNoteAssociation : NSManagedObject

@property (nonatomic, retain) NSString * uuid;
<<<<<<< HEAD
@property (nonatomic, retain) NSString * createdBy;
=======
@property (nonatomic, retain) NSDate * createdBy;
>>>>>>> 2ba7e82d91d3dd1edb435395d6dda9ff888fe580
@property (nonatomic, retain) NSDate * createdDate;
@property (nonatomic, retain) NSString * modifiedBy;
@property (nonatomic, retain) NSDate * modifiedDate;
@property (nonatomic, retain) NSNumber * schemaVersion;
@property (nonatomic, retain) NSNumber * deprecated;
@property (nonatomic, retain) NSNumber * displayOrder;
@property (nonatomic, retain) NSString * linkedNoteId;
@property (nonatomic, retain) NSString * parentId;
@property (nonatomic, retain) NSString * parentEntityName;
<<<<<<< HEAD
@property (nonatomic, retain) NSString * parentEntityPropertyName;
@property (nonatomic, retain) NSNumber * linkedNoteType;

+(NSString *)create;
+(BDLinkedNoteAssociation *)retrieveWithUUID:(NSString *)theUUID;
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary withOverwriteNewerFlag:(BOOL)overwriteNewer; 

-(void)commitChanges;
=======
@property (nonatomic, retain) NSString * parentEntiyPropertyName;
@property (nonatomic, retain) NSNumber * linkedNoteType;

>>>>>>> 2ba7e82d91d3dd1edb435395d6dda9ff888fe580
@end
