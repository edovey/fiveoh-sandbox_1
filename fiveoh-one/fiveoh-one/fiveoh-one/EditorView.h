//
//  EditorView.h
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-17.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface EditorView : UIViewController 
{
    UIWebView *webView;
    UIButton *boldButton;
}
@property (strong, nonatomic) IBOutlet UIButton *boldButton;
@property (strong, nonatomic) IBOutlet UIWebView *webView;
- (IBAction)boldAction:(id)sender;


@end
