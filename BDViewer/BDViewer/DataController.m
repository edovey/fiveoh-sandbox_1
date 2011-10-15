//
//  DataController.m
//  BDViewer
//
//  Created by Karl Schulze on 2011-08-18.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import "DataController.h"

@implementation DataController

@synthesize managedObjectContext = __managedObjectContext;
@synthesize managedObjectModel = __managedObjectModel;
@synthesize persistentStoreCoordinator = __persistentStoreCoordinator;


+ (DataController *)sharedInstance
{
    static dispatch_once_t pred = 0;
    __strong static DataController *_sharedObject = nil;
    
    dispatch_once(&pred, ^{
        _sharedObject = [[self alloc] init]; // or some other init method
    });
    
    return _sharedObject;
}

- (void)saveContext
{
    [self saveContextWithMoc:nil];
}

- (void)saveContextWithMoc:(NSManagedObjectContext *)moc

{
    NSError *error = nil;
    NSManagedObjectContext *managedObjectContext = (nil == moc) ? self.managedObjectContext : moc;
    if (managedObjectContext != nil)
    {
        if ([managedObjectContext hasChanges] && ![managedObjectContext save:&error])
        {
            /*
             Replace this implementation with code to handle the error appropriately.
             
             abort() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development. If it is not possible to recover from the error, display an alert panel that instructs the user to quit the application by pressing the Home button.
             */
            NSLog(@"Unresolved error %@, %@", error, [error userInfo]);
            abort();
        } 
    }
}

- (NSManagedObject *)retrieveManagedObject:(NSString *)entityName 
                                      uuid:(NSString *)uuid 
                                 targetMOC:(NSManagedObjectContext *)moc
{
	if (uuid == nil)
	{
		return nil;
	}
    
    return [self retrieveManagedObjectForValue:entityName 
                                       withKey:@"uuid" 
                                     withValue:uuid 
                                       withMOC:moc];
    
}

- (NSManagedObject *)retrieveManagedObjectForValue:(NSString *)theEntityName 
                                           withKey:(NSString *)theKey 
                                         withValue:(NSString *)theValue 
                                           withMOC:(NSManagedObjectContext *)theMOC
{
   	if (nil == theMOC)
	{
		theMOC = [self managedObjectContext];
	}
	
	NSPredicate *pred;
	pred = [NSPredicate predicateWithFormat:@"(%K == %@)", theKey, theValue];
	
	NSEntityDescription *entity = [NSEntityDescription entityForName:theEntityName inManagedObjectContext:theMOC];
	
	// And, of course, a fetch request. This time we give it both the entity
	// description and the predicate we've just created.
	
	NSFetchRequest *req = [[NSFetchRequest alloc] init];
	[req setEntity:entity];	
	[req setPredicate:pred];
	[req setFetchBatchSize:1];
	
	// We declare an NSError and handle errors by raising an exception,
	// just like in the previous method
	
	NSError *error = nil;
	NSArray *array = [theMOC executeFetchRequest:req
                                           error:&error];  
    [req release];
    
	if (array == nil)
	{
		NSException *exception = [NSException 
								  exceptionWithName:@"MTCoreDataException" /*MTCoreDataExeption*/ 
								  reason:[error localizedDescription] 
								  userInfo:nil];
		[exception raise];
	}
		
	if ([array count] > 0)
	{
		return [array objectAtIndex:0];
	}
	
	return nil;
}

- (NSMutableArray *)allInstancesOf:(NSString *)entityName 
						 orderedBy:(NSString *)orderName 
						  loadData:(BOOL)loadDataFlag
						 targetMOC:(NSManagedObjectContext *)moc
{
	return [self allInstancesOf:entityName 
					  orderedBy:orderName 
					isAscending:YES 
					   loadData:loadDataFlag 
					  targetMOC:moc];
}

- (NSMutableArray *)allInstancesOf:(NSString *)entityName 
						 orderedBy:(NSString *)orderName 
					   isAscending:(BOOL)orderAscending
						  loadData:(BOOL)loadDataFlag
						 targetMOC:(NSManagedObjectContext *)moc
{
	if (moc == nil)
	{
		moc = [self managedObjectContext];
	}
	
    NSFetchRequest *request = [[NSFetchRequest alloc] init];
    
    [request setEntity:[NSEntityDescription entityForName:entityName
								   inManagedObjectContext:moc]];
	
	if(loadDataFlag == YES)
	{
		[request setReturnsObjectsAsFaults:NO];
	}
	
    if (orderName) 
	{
        NSSortDescriptor *sd = [[NSSortDescriptor alloc] initWithKey:orderName
                                                           ascending:orderAscending];
        
        NSArray *sortDescriptors = [NSArray arrayWithObject:sd];
        
        [request setSortDescriptors:sortDescriptors];
        [sd release];
        
    }
    
    NSError *error;
	NSMutableArray *mutableFetchResults = [[moc executeFetchRequest:request error:&error] mutableCopy];
    [request release];
    
	return [mutableFetchResults autorelease];
}

#pragma mark - Core Data stack

/**
 Returns the managed object context for the application.
 If the context doesn't already exist, it is created and bound to the persistent store coordinator for the application.
 */
- (NSManagedObjectContext *)managedObjectContext
{
    if (__managedObjectContext != nil)
    {
        return __managedObjectContext;
    }
    
    NSPersistentStoreCoordinator *coordinator = [self persistentStoreCoordinator];
    if (coordinator != nil)
    {
        __managedObjectContext = [[NSManagedObjectContext alloc] init];
        [__managedObjectContext setPersistentStoreCoordinator:coordinator];
    }
    return __managedObjectContext;
}

/**
 Returns the managed object model for the application.
 If the model doesn't already exist, it is created from the application's model.
 */
- (NSManagedObjectModel *)managedObjectModel
{
    if (__managedObjectModel != nil)
    {
        return __managedObjectModel;
    }
    NSURL *modelURL = [[NSBundle mainBundle] URLForResource:@"fiveoh-one" withExtension:@"momd"];
    __managedObjectModel = [[NSManagedObjectModel alloc] initWithContentsOfURL:modelURL];    
    return __managedObjectModel;
}

/**
 Returns the persistent store coordinator for the application.
 If the coordinator doesn't already exist, it is created and the application's store added to it.
 */
- (NSPersistentStoreCoordinator *)persistentStoreCoordinator
{
    if (__persistentStoreCoordinator != nil)
    {
        return __persistentStoreCoordinator;
    }
    
    NSURL *storeURL = [[self applicationDocumentsDirectory] URLByAppendingPathComponent:@"fiveoh-one.sqlite"];
    
    NSError *error = nil;
    __persistentStoreCoordinator = [[NSPersistentStoreCoordinator alloc] initWithManagedObjectModel:[self managedObjectModel]];
    if (![__persistentStoreCoordinator addPersistentStoreWithType:NSSQLiteStoreType configuration:nil URL:storeURL options:nil error:&error])
    {
        /*
         Replace this implementation with code to handle the error appropriately.
         
         abort() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development. If it is not possible to recover from the error, display an alert panel that instructs the user to quit the application by pressing the Home button.
         
         Typical reasons for an error here include:
         * The persistent store is not accessible;
         * The schema for the persistent store is incompatible with current managed object model.
         Check the error message to determine what the actual problem was.
         
         
         If the persistent store is not accessible, there is typically something wrong with the file path. Often, a file URL is pointing into the application's resources directory instead of a writeable directory.
         
         If you encounter schema incompatibility errors during development, you can reduce their frequency by:
         * Simply deleting the existing store:
         [[NSFileManager defaultManager] removeItemAtURL:storeURL error:nil]
         
         * Performing automatic lightweight migration by passing the following dictionary as the options parameter: 
         [NSDictionary dictionaryWithObjectsAndKeys:[NSNumber numberWithBool:YES], NSMigratePersistentStoresAutomaticallyOption, [NSNumber numberWithBool:YES], NSInferMappingModelAutomaticallyOption, nil];
         
         Lightweight migration will only work for a limited set of schema changes; consult "Core Data Model Versioning and Data Migration Programming Guide" for details.
         
         */
        NSLog(@"Unresolved error %@, %@", error, [error userInfo]);
        abort();
    }    
    
    return __persistentStoreCoordinator;
}

#pragma mark - Application's Documents directory

/**
 Returns the URL to the application's Documents directory.
 */
- (NSURL *)applicationDocumentsDirectory
{
    return [[[NSFileManager defaultManager] URLsForDirectory:NSDocumentDirectory inDomains:NSUserDomainMask] lastObject];
}

@end
