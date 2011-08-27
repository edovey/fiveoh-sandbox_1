//
//  RepositoryHandler.h
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-24.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface RepositoryHandler : NSObject
{
}

+(int)pushQueue;
+(void)pullAll;
+(void)pullLatest;
@end
