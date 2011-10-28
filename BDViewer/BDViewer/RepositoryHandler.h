//
//  RepositoryHandler.h
//  BDViewer
//
//  Created by Karl Schulze on 2011-08-24.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <Foundation/Foundation.h>

#define REPOSITORY_REFRESHDATE_KEY @"RepositoryRefreshDate"

@interface RepositoryHandler : NSObject
{
}

+(int)pushQueue;
+(void)pullSince:(NSDate *)theLastSyncDate;
@end
