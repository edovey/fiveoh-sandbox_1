//
//  EditorView.h
//  fiveoh-one
//
//  Created by Karl Schulze on 2011-08-17.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <UIKit/UIKit.h>
@class EditorDocument;

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
    EditorDocument      *editorDocument;
    
}

@property (strong, nonatomic) EditorDocument    *editorDocument;

@property (strong, nonatomic) IBOutlet UIButton *orderedListButton;
@property (strong, nonatomic) IBOutlet UIButton *unorderedListButton;
@property (strong, nonatomic) IBOutlet UIButton *boldButton;
@property (strong, nonatomic) IBOutlet UIButton *indentButton;
@property (strong, nonatomic) IBOutlet UIButton *outdentButton;
@property (strong, nonatomic) IBOutlet UIButton *saveButton;
@property (strong, nonatomic) IBOutlet UISwitch *settingsSwitch;
@property (strong, nonatomic) IBOutlet UIWebView *webView;

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
