//
//  TapDetectingWindow.m
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-17.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//
// http://mithin.in/2009/08/26/detecting-taps-and-events-on-uiwebview-the-right-way/
//

#import "TapDetectingWindow.h"
@implementation TapDetectingWindow
@synthesize viewToObserve;
@synthesize controllerThatObserves;

- (id)initWithViewToObserver:(UIView *)view andDelegate:(id)delegate 
{
    self = [super init];
    if (self != nil)
    {
        self.viewToObserve = view;
        self.controllerThatObserves = delegate;
    }
    return self;
}

- (void)forwardTap:(id)touch 
{
    [controllerThatObserves userDidTapWebView:touch];
}

- (void)sendEvent:(UIEvent *)event 
{
    [super sendEvent:event];
    
    if (viewToObserve == nil || controllerThatObserves == nil)
        return;
    
    NSSet *touches = [event allTouches];
    if (touches.count != 1)
        return;
    
    UITouch *touch = touches.anyObject;
    if (touch.phase != UITouchPhaseEnded)
        return;
    
    if ([touch.view isDescendantOfView:viewToObserve] == NO)
        return;
    
    CGPoint tapPoint = [touch locationInView:viewToObserve];
    NSLog(@"TapPoint = %f, %f", tapPoint.x, tapPoint.y);
    NSArray *pointArray = [NSArray arrayWithObjects:[NSString stringWithFormat:@"%f", tapPoint.x],
                           [NSString stringWithFormat:@"%f", tapPoint.y], nil];
    if (touch.tapCount == 1) 
    {
        [self performSelector:@selector(forwardTap:) withObject:pointArray afterDelay:0.5];
    }
    else if (touch.tapCount > 1) 
    {
        [NSObject cancelPreviousPerformRequestsWithTarget:self selector:@selector(forwardTap:) object:pointArray];
    }
}
@end
