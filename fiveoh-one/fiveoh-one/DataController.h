//
//  DataController.h
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-18.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>

@interface DataController : NSObject

@property (readonly, strong, nonatomic) NSManagedObjectContext *managedObjectContext;
@property (readonly, strong, nonatomic) NSManagedObjectModel *managedObjectModel;
@property (readonly, strong, nonatomic) NSPersistentStoreCoordinator *persistentStoreCoordinator;

+ (DataController *)sharedInstance;
- (void)saveContext;
- (void)saveContextWithMoc:(NSManagedObjectContext *)moc;
- (NSManagedObject *)retrieveManagedObject:(NSString *)entityName 
                                      uuid:(NSString *)uuid 
                                 targetMOC:(NSManagedObjectContext *)moc;

- (NSManagedObject *)retrieveManagedObjectForValue:(NSString *)theEntityName 
                                           withKey:(NSString *)theKey 
                                         withValue:(NSString *)theValue 
                                           withMOC:(NSManagedObjectContext *)theMOC;

- (NSMutableArray *)allInstancesOf:(NSString *)entityName 
						 orderedBy:(NSString *)orderName 
						  loadData:(BOOL)loadDataFlag
						 targetMOC:(NSManagedObjectContext *)moc;

- (NSMutableArray *)allInstancesOf:(NSString *)entityName 
						 orderedBy:(NSString *)orderName 
					   isAscending:(BOOL)orderAscending
						  loadData:(BOOL)loadDataFlag
						 targetMOC:(NSManagedObjectContext *)moc;

- (NSURL *)applicationDocumentsDirectory;

@end
