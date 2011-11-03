//
//  DiseaseView.h
//  BDViewer
//
//  Created by Liz Dovey on 11-10-19.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface DiseaseListView : UIViewController <UITableViewDelegate, UITableViewDataSource, UIWebViewDelegate>

@property (nonatomic, retain) NSArray * diseaseArray;
@property (retain, nonatomic) NSString *parentId;
@property (retain, nonatomic) IBOutlet UITableView *dataTableView;
@property (nonatomic, retain) NSString *parentName;

-(id)initWithParentId:(NSString *)pParentId withParentName:(NSString *)pParentName;
@end
