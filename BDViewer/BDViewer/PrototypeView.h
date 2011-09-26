//
//  PrototypeView.h
//  BDViewer
//
//  Created by Karl Schulze on 2011-08-26.
//  Copyright (c) 2011 TLA Digital Projects. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface PrototypeView : UIViewController 
{
    NSMutableArray *_documentArray;
    UITableView *dataTableView;
    UILabel *infoLabel;
    UILabel *modifiedDateLabel;
    UITextField *repositoryUrlTextField;
    UITextView *documentTextTextView;
}

@property (nonatomic, retain) NSMutableArray *documentArray;
@property (nonatomic, retain) IBOutlet UITableView *dataTableView;
@property (nonatomic, retain) IBOutlet UILabel *infoLabel;
@property (nonatomic, retain) IBOutlet UILabel *modifiedDateLabel;
@property (nonatomic, retain) IBOutlet UITextField *repositoryUrlTextField;
@property (nonatomic, retain) IBOutlet UITextView *documentTextTextView;

- (IBAction)updateRowAction:(id)sender;
- (IBAction)pushAction:(id)sender;
- (IBAction)pullAction:(id)sender;
- (IBAction)createAction:(id)sender;

@end
