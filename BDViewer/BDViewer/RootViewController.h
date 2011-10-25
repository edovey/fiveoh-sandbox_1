//
//  RootViewController.h
//  tmp
//
//  Created by Liz Dovey on 11-09-23.
//  Copyright 2011 875953 Alberta, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface RootViewController : UIViewController
{
    UIActivityIndicatorView *syncActivityIndicator;
}

@property (retain, nonatomic) IBOutlet UIActivityIndicatorView *syncActivityIndictor;

-(IBAction)openTouched:(id)sender;
-(IBAction)loadTouched:(id)sender;
@end
