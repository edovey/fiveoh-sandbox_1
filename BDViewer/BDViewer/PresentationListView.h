//
//  PresentationView.h
//  BDViewer
//
//  Created by Liz Dovey on 11-10-20.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface PresentationListView : UIViewController <UITableViewDelegate, UITableViewDataSource, UIWebViewDelegate>

@property (retain, nonatomic) NSString *parentId;
@property (nonatomic, retain) NSString *parentName;
@property (retain, nonatomic) NSArray *presentationArray;
@property (retain, nonatomic) IBOutlet UITableView *dataTableView;
@property (retain, nonatomic) IBOutlet UIWebView *dataWebView;
@property (retain, nonatomic) NSString * overviewHTMLString;

-(id)initWithParentId:(NSString *)pParentId withParentName:(NSString *)pParentName;
@end
