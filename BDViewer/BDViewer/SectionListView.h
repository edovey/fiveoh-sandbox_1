//
//  SectionListView.h
//  BDViewer
//
//  Created by Liz Dovey on 11-10-19.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SectionListView : UIViewController <UITableViewDataSource, UITableViewDelegate>

@property (nonatomic, retain) IBOutlet UITableView *dataTableView;
@property (nonatomic, retain) NSString *parentId;
@property (nonatomic, retain) NSString *parentName;
@property (nonatomic, retain) NSArray *sectionArray;

-(id)initWithParentId:(NSString *)pParentId withParentName:(NSString *)pParentName;

@end
