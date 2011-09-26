//
//  BDViewerAppDelegate.h
//  BDViewer
//
//  Created by Liz Dovey on 11-09-21.
//  Copyright 2011 875953 Alberta, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@class RootViewController;

@interface BDViewerAppDelegate : NSObject <UIApplicationDelegate>

@property (nonatomic, retain) IBOutlet UIWindow *window;
@property (nonatomic, retain) IBOutlet UINavigationController *navigationController;

- (NSURL *)applicationDocumentsDirectory;


@end
