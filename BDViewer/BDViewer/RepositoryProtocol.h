//
//  AWS_RepositoryProtocol.h
//  BDViewer
//
//  Created by Karl Schulze on 2011-10-20.
//  Copyright 2011 875953 Alberta, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol RepositoryProtocol <NSObject>

//Update or create using an attribute dictionary. key=(aws)property name value=string representation of property value
//Return the uuid of the instance if successful, null if a conflict prevented the load
+(NSString *)loadWithAttributes:(NSDictionary *)theAttributeDictionary 
         withOverwriteNewerFlag:(BOOL)overwriteNewer; 

//Return a dictionary of attributes for the instance. key=(aws)property name value=string representation of property value
-(NSDictionary *)propertyAttributeDictionary;

@end
