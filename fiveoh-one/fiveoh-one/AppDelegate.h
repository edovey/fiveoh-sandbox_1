//
//  AppDelegate.h
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-15.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <UIKit/UIKit.h>

@class ViewController;

@interface AppDelegate : UIResponder <UIApplicationDelegate>

@property (strong, nonatomic) UIWindow                  *window;
@property (strong, nonatomic) UINavigationController    *navigationController;

@property (strong, nonatomic) ViewController            *viewController;

@end
