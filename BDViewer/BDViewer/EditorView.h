//
//  EditorView.h
//  BDViewer
//
//  Created by Karl Schulze on 2011-08-17.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <UIKit/UIKit.h>
@class BDLinkedNote;

@interface EditorView : UIViewController 
{
    UIWebView           *webView;
    UIButton            *boldButton;
    UIButton            *unorderedListButton;
    UIButton            *indentButton;
    UIButton            *outdentButton;
    UIButton            *saveButton;
    UISwitch *settingsSwitch;
    UIButton            *orderedListButton;
    
    @private
    BDLinkedNote      *editorDocument;
    BOOL keyboardIsShown;

}

@property (nonatomic, retain) BDLinkedNote    *editorDocument;

@property (nonatomic, retain) IBOutlet UIButton *orderedListButton;
@property (nonatomic, retain) IBOutlet UIButton *unorderedListButton;
@property (nonatomic, retain) IBOutlet UIButton *boldButton;
@property (nonatomic, retain) IBOutlet UIButton *indentButton;
@property (nonatomic, retain) IBOutlet UIButton *outdentButton;
@property (nonatomic, retain) IBOutlet UIButton *saveButton;
@property (nonatomic, retain) IBOutlet UISwitch *settingsSwitch;
@property (nonatomic, retain) IBOutlet UIWebView *webView;

- (void)assignDocumentUuid:(NSString *)theUuid;
- (NSArray *)generateMenuItems;

- (IBAction)boldAction:(id)sender;
- (IBAction)orderedListAction:(id)sender;
- (IBAction)unorderedListAction:(id)sender;
- (IBAction)indentAction:(id)sender;
- (IBAction)outdentAction:(id)sender;
- (IBAction)saveAction:(id)sender;
- (IBAction)settingsToggle:(id)sender;


@end
