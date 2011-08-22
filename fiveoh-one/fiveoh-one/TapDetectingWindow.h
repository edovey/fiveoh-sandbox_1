//
//  TapDetectingWindow.h
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-17.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//
// http://mithin.in/2009/08/26/detecting-taps-and-events-on-uiwebview-the-right-way/
//


#import <UIKit/UIKit.h>

@protocol TapDetectingWindowDelegate
- (void)userDidTapWebView:(id)tapPoint;
@end

@interface TapDetectingWindow : UIWindow 
{
    UIView *viewToObserve;
    id <TapDetectingWindowDelegate> controllerThatObserves;
}
@property (nonatomic, retain) UIView *viewToObserve;
@property (nonatomic, retain) id <TapDetectingWindowDelegate> controllerThatObserves;
@end