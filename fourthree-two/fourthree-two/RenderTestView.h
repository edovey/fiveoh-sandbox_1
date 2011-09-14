//
//  RenderTestView.h
//  fourthree-two
//
//  Created by Karl Schulze on 11-09-12.
//  Copyright 2011 TLA Digital Projects. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <CoreText/CoreText.h>
@class UIPrintInteractionController;

@interface RenderTestView : UIViewController
{
    UIPrintInteractionController            *printController;
    UIWebView                               *webView;
}
@property (nonatomic, retain) IBOutlet UIWebView *webView;

- (IBAction)outputTestOneAction:(id)sender;
- (IBAction)outputTestTwoAction:(id)sender;

@end
